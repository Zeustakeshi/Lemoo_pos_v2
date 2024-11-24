namespace Lemoo_pos.Common.Enums
{
    public enum PermissionType
    {
        // Manage staff
        VIEW_ALL_STAFF,
        IMPORT_STAFF_FILE,
        EXPORT_STAFF_FILE,
        ADD_STAFF,
        EDIT_STAFF,
        DELETE_STAFF,

        // Manage product
        VIEW_ALL_PRODUCT,
        IMPORT_PRODUCT_FILE,
        EXPORT_PRODUCT_FILE,
        ADD_PRODUCT,
        EDIT_PRODUCT,
        DELETE_PRODUCT,

        // Manage store
        ASSIGN_ROLE,
        MANAGE_BRANCH,
        VIEW_STORE_INFO,
        VIEW_ACTIVITY_LOG,

        // Manage customer
        VIEW_ASSIGNED_CUSTOMER,
        VIEW_ALL_CUSTOMER,
        ADD_CUSTOMER,
        EDIT_CUSTOMER,
        DELETE_CUSTOMER,
        IMPORT_CUSTOMER_FILE,

        // Manage order
        VIEW_ASSIGNED_ORDER,
        VIEW_ALL_ORDER,
        ADD_ORDER,
        EDIT_ORDER,
        APPROVE_ORDER,
        CANCEL_ORDER,
        EXPORT_ORDER_FILE,
        IMPORT_ORDER_FILE,

        // Mange inventory
        VIEW_AUDIT_REPORT,
        CREATE_AUDIT_REPORT,
        DELETE_AUDIT_REPORT,
        EDIT_AUDIT_REPORT,
        BALANCE_WAREHOUSE,
        EXPORT_AUDIT_FILE,
        IMPORT_AUDIT_FILE
    }
}
