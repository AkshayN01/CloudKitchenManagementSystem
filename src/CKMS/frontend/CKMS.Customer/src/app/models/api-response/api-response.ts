// http-response.model.ts
export interface HTTPResponse<T> {
    data: T;
    meta: HTTPMeta;
}
  
export interface HTTPMeta {
    retVal: number;
    message: string;
}
  