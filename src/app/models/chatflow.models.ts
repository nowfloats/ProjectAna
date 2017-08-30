export interface ANAProject {
    Name: string;
    CreatedOn: Date;
    UpdatedOn: Date;
    _id: string;
}

export interface Section {
    Text: string;
    SectionType: number;
    DelayInMs: number;
    Hidden: boolean;
    Alias: string;
    _id: string;
    Title: string;
    Caption: string;
    AltText?: any;
    HeightInPixels?: number;
    WidthInPixels?: number;
    Url: string;
    SizeInKb?: number;
}

export interface Button {
    ButtonName?: any;
    ButtonText: string;
    Emotion: number;
    ButtonType: number;
    DeepLinkUrl?: any;
    Url: string;
    BounceTimeout?: any;
    NextNodeId: string;
    DefaultButton: boolean;
    Hidden: boolean;
    VariableValue?: any;
    PrefixText?: any;
    PostfixText?: any;
    PlaceholderText?: any;
    APIResponseMatchKey?: any;
    APIResponseMatchValue?: any;
    PostToChat: boolean;
    DoesRepeat: boolean;
    RepeatOn?: any;
    RepeatAs?: any;
    StartPosition: number;
    MaxRepeats: number;
    Alias: string;
    _id: string;
}

export interface ChatNode {
    Name: string;
    Id: string;
    Emotion: number;
    TimeoutInMs: number;
    NodeType: number;
    Sections: Section[];
    Buttons: Button[];
    VariableName?: any;
    ApiMethod?: any;
    ApiUrl?: any;
    ApiResponseDataRoot?: any;
    NextNodeId?: any;
    RequiredVariables?: any;
    GroupName?: any;
    FlowId?: any;
    CardHeader?: any;
    CardFooter?: any;
    Placement?: any;
}

export interface ChatContent {
    ButtonId: string;
    ButtonText: string;
    NodeName?: string;
    NodeId: string;
    Emotion?: string;
    CreatedOn: Date;
    UpdatedOn: Date;
    _id?: string;
    SectionText: string;
    SectionId: string;
    Title: string;
    Caption: string;
}

export interface ChatFlowPack {
    ProjectId: string;
    ChatNodes: ChatNode[];
    ChatContent: ChatContent[];
    NodeLocations: any;
    CreatedOn: Date;
    UpdatedOn: Date;
    _id: string;
}