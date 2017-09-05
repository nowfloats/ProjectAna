export interface ANAProject {
    Name: string;
    CreatedOn: Date;
    UpdatedOn: Date;
    _id: string;
}

//Enum Start
export enum SectionType {
    Image = 'Image',
    Text = 'Text',
    Graph = 'Graph',
    Gif = 'Gif',
    Audio = 'Audio',
    Video = 'Video',
    Link = 'Link',
    EmbeddedHtml = 'EmbeddedHtml',
    Carousel = 'Carousel',
    PrintOTP = 'PrintOTP'
}

export enum CarouselButtonType {
    NextNode = 'NextNode',
    DeepLink = 'DeepLink',
    OpenUrl = 'OpenUrl'
}

export enum NodeType {
    ApiCall = 'ApiCall',
    Combination = 'Combination',
    Card = 'Card'
}

export enum APIMethod {
    GET = 'GET',
    POST = 'POST',
}

export enum CardPlacement {
    Incoming = 'Incoming',
    Outgoing = 'Outgoing',
    Center = 'Center'
}
//Enums End

// Sections - Start
export interface TitleCaptionEntity {
    Title: string;
    Caption: string;
}

export abstract class BaseIdEntity {
    _id: string;
}

export abstract class BaseEntity extends BaseIdEntity {
    abstract Alias(): string;
}

export class Section extends BaseEntity {
    SectionType: SectionType;
    DelayInMs: number;
    Hidden: boolean;

    Alias(): string {
        return this.SectionType;
    }
}

export abstract class RepeatableBaseEntity extends BaseEntity {
    DoesRepeat: boolean;
    RepeatOn: string;
    RepeatAs: string;
    StartPosition: string;
    MaxRepeats: number;
}

export class TextSection extends Section {
    constructor() {
        super();
        this.SectionType = SectionType.Text;
    }

    Text: string;

    Alias(): string {
        return this.Text || this.SectionType;
    }
}

export abstract class TitleCaptionSection extends Section implements TitleCaptionEntity {
    Title: string;
    Caption: string;

    Alias(): string {
        return this.Title || this.Caption || this.SectionType;
    }
}

export abstract class TitleCaptionUrlSection extends TitleCaptionSection {
    Url: string;

    Alias(): string {
        return this.Title || this.Caption || this.SectionType;
    }
}

export class ImageSection extends TitleCaptionUrlSection {
    constructor() {
        super();
        this.SectionType = SectionType.Image;
    }
}

export class VideoSection extends TitleCaptionUrlSection {
    constructor() {
        super();
        this.SectionType = SectionType.Video;
    }
}

export class AudioSection extends TitleCaptionUrlSection {
    constructor() {
        super();
        this.SectionType = SectionType.Audio;
    }
}

export class EmbeddedHtmlSection extends TitleCaptionUrlSection {
    constructor() {
        super();
        this.SectionType = SectionType.EmbeddedHtml;
    }
}

export class CarouselButton extends RepeatableBaseEntity {

    Url: string;
    Type: CarouselButtonType;
    VariableValue: string;
    NextNodeId: string;
    Text: string;

    Alias(): string {
        return this.Text || this.Type;
    }
}

export class CarouselItem extends RepeatableBaseEntity implements TitleCaptionEntity {
    Title: string;
    Caption: string;

    Alias(): string {
        return this.Title || this.Caption || 'Carousel Item';
    }
}

export class CarouselSection extends TitleCaptionSection {
    constructor() {
        super();
        this.SectionType = SectionType.Carousel;
    }

    Items: CarouselItem[];
}
// Sections - End

export enum ButtonType {
    PostText = 'PostText',
    OpenUrl = 'OpenUrl',
    GetText = 'GetText',
    GetNumber = 'GetNumber',
    GetAddress = 'GetAddress',
    GetEmail = 'GetEmail',
    GetPhoneNumber = 'GetPhoneNumber',
    GetItemFromSource = 'GetItemFromSource',
    GetImage = 'GetImage',
    GetAudio = 'GetAudio',
    GetVideo = 'GetVideo',
    NextNode = 'NextNode',
    DeepLink = 'DeepLink',
    GetAgent = 'GetAgent',
    ShowConfirmation = 'ShowConfirmation',
    FetchChatFlow = 'FetchChatFlow',
    /// Format: yyyy-MM-dd
    GetDate = 'GetDate',
    /// Format: HH:mm:ss
    GetTime = 'GetTime',
    /// Format: yyyy-MM-ddTHH:mm:ss
    GetDateTime = 'GetDateTime',
    /// Format: [Latitude],[Longitude]
    GetLocation = 'GetLocation'
}

export class Button implements BaseIdEntity {
    ButtonName: string;
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

export class ChatNode {
    Name: string;
    Id: string;
    Emotion: string;
    TimeoutInMs: number;
    NodeType: NodeType;
    Sections: Section[];
    Buttons: Button[];
    VariableName?: string;
    ApiMethod?: APIMethod;
    ApiUrl?: string;
    ApiResponseDataRoot?: string;
    NextNodeId?: string;
    RequiredVariables?: string[];
    GroupName?: string;
    CardHeader?: string;
    CardFooter?: string;
    Placement?: CardPlacement;
    IsStartNode: boolean;
}

export class ChatContent {
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

export class ChatFlowPack {
    ProjectId: string;
    ChatNodes: ChatNode[];
    ChatContent: ChatContent[];
    NodeLocations: any;
    CreatedOn: Date;
    UpdatedOn: Date;
    _id: string;
}

export enum EditorType {
    Text = 'Text',
    TitleCaptionUrl = 'TitleCaptionUrl',
    Carousel = 'Carousel'
}
