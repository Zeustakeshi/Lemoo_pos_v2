importScripts("https://cdn.jsdelivr.net/npm/dexie@3.2.5/dist/dexie.min.js");
importScripts(
    "https://cdnjs.cloudflare.com/ajax/libs/axios/1.7.8/axios.min.js"
);

let accessToken = undefined;

function openDatabase() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open("LemooDatabase");
        request.onsuccess = (event) => resolve(event.target.result);
        request.onerror = (event) => reject(event.target.error);
    });
}

function chunkArray(array, size) {
    const chunks = [];
    for (let i = 0; i < array.length; i += size) {
        chunks.push(array.slice(i, i + size));
    }
    return chunks;
}

async function sleep(ms) {
    return new Promise((rs) => {
        setTimeout(rs, ms);
    });
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
            console.log(
                `[SW] Order with ID ${id} has been removed from IndexedDB.`
            );
            resolve();
        };

        request.onerror = (event) => {
            console.error(
                "[SW] Error while deleting order:",
                event.target.error
            );
            reject(event.target.error);
        };
    });
}

async function insertSyncOrderLog() {}

async function syncOrders() {
    try {
        const orders = await getAllOrders();

        if (orders.length === 0) {
            console.log("[SW] No orders to synchronize.");
            return;
        }

        if (!accessToken) {
            console.log("[SW] Unauthorized to synchronize.");
        }

        const chunks = chunkArray(orders, 50);

        for (const chunk of chunks) {
            try {
                const { data } = await axios.post("/api/orders/batch", chunk, {
                    headers: {
                        Authorization: `Bearer ${accessToken}`,
                        "Content-Type": "application/json",
                    },
                });

                for (const order of chunk) {
                    await deleteOrder(order.id);
                }

                console.log(
                    `[SW] Successfully synchronized a batch of ${chunk.length} orders!`
                );
            } catch (error) {
                console.error("[SW] Synchronization failed!");
                throw error;
            }
        }
        console.log("[SW] Completed synchronization of all orders.");
    } catch (error) {
        console.error("[SW] Error during synchronization:", error);
    }
}

self.addEventListener("sync", (event) => {
    if (event.tag === "sync-orders") {
        console.log("[SW] Processing Order synchronization...");
        event.waitUntil(syncOrders());
    }
});

self.addEventListener("message", (event) => {
    console.log("[SW] Received message:", event.data);

    if (event.data && event.data.type === "SEND_ACCESS_TOKEN") {
        accessToken = event.data.message;
    }
});

self.addEventListener("install", (event) => {
    console.log("[SW] Service Worker: Installed");

    // Automatically activate right after installation
    event.waitUntil(self.skipWaiting());
});

self.addEventListener("activate", (event) => {
    console.log("[SW] Service Worker: Activated");

    event.waitUntil(self.clients.claim());
});
