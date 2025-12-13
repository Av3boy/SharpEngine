export type Asset = {
    id : string,
    name : string,
    description : string,
    price : number,
    createdAt : string,
    lastUpdatedAt : string,
    keyWords : string[],
    comments : AssetComment[],
    authorId : string,
    tombstoned : boolean,
    version : number,
    blobUri : string,
    fileSizeBytes : number,
    checksum : string,
};

export type AssetComment = {
}