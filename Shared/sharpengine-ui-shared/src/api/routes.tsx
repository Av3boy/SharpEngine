// TODO: This should be moved to environment variables later since the values may change between environments
export const Routes =
{
    AssetStore: "http://localhost:3000",
    Portal: "http://localhost:3001",
}

export const AssetRoutes =
{
    AssetStore: `${Routes.AssetStore}/api/v1/assets`,
}

export const UserRoutes =
{
    Portal: `${Routes.Portal}/api/v1/users`,
}

export const CommentRoutes =
{
    AssetStore: `${Routes.AssetStore}/api/v1/comments`,
}