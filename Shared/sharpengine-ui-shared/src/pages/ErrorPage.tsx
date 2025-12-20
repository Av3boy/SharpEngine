export default function ErrorPage({location}: any) {
    const raw = location.search || location.hash || "";
    const cleaned = raw.replace("#", "").split("?").slice(1).join("?");
    const searchParams = new URLSearchParams(cleaned);

    const errorMessage = searchParams.get("errorMessage") ?? "The page you are looking for does not exist.";
    const qpStatus = searchParams.get("statusCode");
    const statusCode = qpStatus ? Number.parseInt(qpStatus) : 404;

    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-gray-100">
            <h1 className="text-5xl font-bold mb-4">{statusCode}</h1>
            <p className="text-xl mb-8">{errorMessage}</p>
            <a href="/" className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">Go to Home</a>
        </div>
    );
}