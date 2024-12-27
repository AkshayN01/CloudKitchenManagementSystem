export interface CustomerDTO{
    customerId: string,
    name: string,
    phoneNumber: string,
    userName: string,
    emailId: string,
    loyaltyPoints: number,
    totalOrder: number
}

export interface AddressDTO{
    addressId: string,
    addressDetail: string,
    city: string,
    region: string,
    postalCode: string,
    country: string
}

export interface LoginResponse{
    name: string,
    token: string
}