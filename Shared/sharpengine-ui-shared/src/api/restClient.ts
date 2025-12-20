export type HttpMethod = "GET" | "POST" | "PUT" | "DELETE" | "PATCH";

export interface RestClientOptions {
    method?: HttpMethod;
    headers?: Record<string, string>;
    contentType?: string;   // default: "application/json"
    body?: any;
}

export interface RestClientResult<T> {
    isSuccess: boolean;
    status: number;
    data: T;
}

// 1) Overload: returns typed data (default string)
export function apiRequest<T = string>(
    url: string,
    options?: RestClientOptions
): Promise<RestClientResult<T>>;

// 2) Overload: no return body (void)
export function apiRequest(
    url: string,
    options?: RestClientOptions
): Promise<RestClientResult<void>>;

export async function apiRequest<T = string>(
    url: string,
    options: RestClientOptions = {}
): Promise<RestClientResult<T>> {

    const {
        method = "GET",
        contentType = "application/json",
        headers = {},
        body
    } = options;

    // Build final headers
    const finalHeaders: Record<string, string> = {
        ...(contentType ? { "Content-Type": contentType } : {}),
        ...headers,
    };

    const response = await fetch(url, {
        method,
        headers: finalHeaders,
        body: body ? JSON.stringify(body) : undefined,
    });

    const isSuccess = response.ok;
    const status = response.status;

    // Handle no-content responses (204, 205)
    if (status === 204 || status === 205) {
        return { isSuccess, status, data: undefined as T };
    }

    // Try JSON first (if content-type is JSON)
    let data: any;

    const responseContentType = response.headers.get("Content-Type") ?? "";

    if (responseContentType.includes("application/json")) {
        data = (await response.json()) as T;
    } else {
        // fallback to text
        data = (await response.text()) as T;
    }

    return {
        isSuccess,
        status,
        data,
    };
}