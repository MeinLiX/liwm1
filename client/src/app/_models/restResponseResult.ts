export interface RestResponseResult {
    is_success: boolean;
    date: Date;
    status_code: number;
    message: string;
}

export interface DataRestResponseResult<T> extends RestResponseResult {
    data?: T;
}