import { AdminUserPayload } from "./adminUser";

export interface RegisterPayload{
    kitchenName: string,
    address: string,
    city: string,
    region: string,
    postalCode: string,
    country: string,
    adminUserPayload: AdminUserPayload
}
