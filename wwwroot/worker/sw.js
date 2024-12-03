importScripts("https://cdn.jsdelivr.net/npm/dexie@3.2.5/dist/dexie.min.js");

function openDatabase() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open("LemooDatabase");
        request.onsuccess = (event) => resolve(event.target.result);
        request.onerror = (event) => reject(event.target.error);
    });
}

async function syncOrders() {
    try {
        const orders = await getAllOrders();
        console.log({ orders });
        if (orders.length === 0) {
            console.log("[SW] Không có đơn hàng để đồng bộ.");
            return;
        }

        // Gửi đơn hàng lên server qua API batch
        const response = await fetch("/api/pos/orders/batch", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(orders.map((order) => order.data)),
        });

        if (response.ok) {
            console.log("[SW] Đồng bộ thành công!");
            // Xóa các đơn hàng đã được đồng bộ
            for (const order of orders) {
                await deleteOrder(order.id);
            }
        } else {
            console.error("[SW] Đồng bộ thất bại!", await response.text());
        }
    } catch (error) {
        console.error("[SW] Lỗi khi đồng bộ:", error);
    }
}

async function getAllOrders() {
    const db = await openDatabase();
    return new Promise((resolve, reject) => {
        const transaction = db.transaction("orders", "readonly");
        const store = transaction.objectStore("orders");

        const request = store.getAll();

        request.onsuccess = (event) => resolve(event.target.result);
        request.onerror = (event) => reject(event.target.error);
    });
}

async function deleteOrder(id) {
    const db = await openDatabase();
    return new Promise((resolve, reject) => {
        const transaction = db.transaction("orders", "readwrite");
        const store = transaction.objectStore("orders");

        const request = store.delete(id);

        request.onsuccess = () => {
            console.log(`[SW] Đã xóa đơn hàng có ID ${id} khỏi IndexedDB.`);
            resolve();
        };

        request.onerror = (event) => {
            console.error("[SW] Lỗi khi xóa đơn hàng:", event.target.error);
            reject(event.target.error);
        };
    });
}

self.addEventListener("sync", (event) => {
    if (event.tag === "sync-orders") {
        console.log("[SW] Đang xử lý đồng bộ...");
        event.waitUntil(syncOrders());
    }
});
