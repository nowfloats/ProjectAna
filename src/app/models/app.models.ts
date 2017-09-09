export interface ChatServerConnection {
    Name: string;
    ServerUrl: string;
    APIKey: string;
    APISecret: string;
    IsDefault: boolean;
}

export interface ANAProject {
    Name: string;
    CreatedOn: Date;
    UpdatedOn: Date;
    _id: string;
}
