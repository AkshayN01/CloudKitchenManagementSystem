export interface RegisterPayload{
    name: string,
    phoneNumber: string,
    emailId: string,
    password: string
}

export interface LoginPayload{
    userName: string,
    password: string
}

export interface CustomerUpdatePayload{
    name: string,
    phoneNumber: string,
    emailId: string,
    password: string
}

export interface AddressPayload{
    addressDetail: string,
    city: string,
    region: string,
    postalCode: string,
    country: string
}

export interface AddressUpdatePayload{
    addressId: string,
    addressDetail: string,
    city: string,
    region: string,
    postalCode: string,
    country: string
}