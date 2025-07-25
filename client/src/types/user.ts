// this is in the camel case and in the API side we are using the pascal case, reason we are using here the camel case is the data from the API is returned in the JSON format and the JSON is in the camel case.
export type User = {
    id: string;
    displayName: string;
    email: string;
    token: string;
    imageUrl?: string;
}

export type LoginCreds = {
    email: string;
    password: string;
}

export type RegisterCreds = {
    email: string;
    displayName: string;
    password: string;
}