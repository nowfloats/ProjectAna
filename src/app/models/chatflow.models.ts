import { ObjectID } from 'bson';
import { ChatFlowComponent, ChatNodeVM } from '../components/chatflow/chatflow.component';
import { GlobalsService } from '../services/globals.service';
import { MdButton } from '@angular/material';

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

export interface BaseIdEntity {
    _id: string;
}

export interface BaseEntity extends BaseIdEntity { }

export interface Section extends BaseEntity {
    SectionType: SectionType;
    DelayInMs?: number;
    Hidden?: boolean;
}

export interface RepeatableBaseEntity extends BaseEntity {
    DoesRepeat: boolean;
    RepeatOn: string;
    RepeatAs: string;
    StartPosition: string;
    MaxRepeats: number;
}

export interface TextSection extends Section {
    Text: string;
}

export interface TitleCaptionSection extends Section, TitleCaptionEntity { }

export interface TitleCaptionUrlSection extends TitleCaptionSection {
    Url: string;
}

export interface ImageSection extends TitleCaptionUrlSection { }

export interface VideoSection extends TitleCaptionUrlSection { }

export interface AudioSection extends TitleCaptionUrlSection { }

export interface EmbeddedHtmlSection extends TitleCaptionUrlSection { }

export interface CarouselButton extends RepeatableBaseEntity {
    Url: string;
    Type: CarouselButtonType;
    VariableValue: string;
    NextNodeId: string;
    Text: string;
}

export interface CarouselItem extends RepeatableBaseEntity, TitleCaptionEntity {
    ImageUrl: string;
    Buttons: CarouselButton[];
}

export interface CarouselSection extends TitleCaptionSection {
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
    GetFile = 'GetFile',
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

export interface Button extends BaseIdEntity {
    ButtonName?: string;
    ButtonText: string;
    Emotion?: number;
    ButtonType: ButtonType;
    DeepLinkUrl?: string;
    Url?: string;
    BounceTimeout?: number;
    NextNodeId?: string;
    DefaultButton?: boolean;
    Hidden?: boolean;
    VariableValue?: string;
    PrefixText?: string;
    PostfixText?: string;
    PlaceholderText?: string;
    APIResponseMatchKey?: string;
    APIResponseMatchValue?: string;
    PostToChat?: boolean;
    DoesRepeat?: boolean;
    RepeatOn?: string;
    RepeatAs?: string;
    StartPosition?: number;
    MaxRepeats?: number;
    AdvancedOptions?: boolean;
}

export interface ChatNode {
    Name: string;
    Id: string;
    Emotion?: string;
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
    IsStartNode?: boolean;
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
    NodeLocations: NodeLocations;
    CreatedOn: Date;
    UpdatedOn: Date;
    _id: string;
}

export interface NodeLocations {
    [key: string]: LayoutPoint;
}

export interface LayoutPoint {
    X: number;
    Y: number;
}

export enum EditorType {
    Text = 'Text',
    TitleCaptionUrl = 'TitleCaptionUrl',
    Carousel = 'Carousel'
}

export class ModelHelpers {
    constructor(
        public globalsService: GlobalsService) {

    }

    nodeTypes: NodeType[] = [
        NodeType.ApiCall,
        NodeType.Combination,
        NodeType.Card,
    ];
    apiMethods: APIMethod[] = [
        APIMethod.GET,
        APIMethod.POST,
    ];
    cardPlacements: CardPlacement[] = [
        CardPlacement.Center,
        CardPlacement.Incoming,
        CardPlacement.Outgoing,
    ];
    buttonTypes: ButtonType[] = [
        ButtonType.DeepLink,
        ButtonType.FetchChatFlow,
        ButtonType.GetAddress,
        ButtonType.GetAgent,
        ButtonType.GetAudio,
        ButtonType.GetDate,
        ButtonType.GetDateTime,
        ButtonType.GetEmail,
        ButtonType.GetImage,
        ButtonType.GetItemFromSource,
        ButtonType.GetLocation,
        ButtonType.GetNumber,
        ButtonType.GetPhoneNumber,
        ButtonType.GetText,
        ButtonType.GetTime,
        ButtonType.GetVideo,
        ButtonType.GetFile,
        ButtonType.NextNode,
        ButtonType.OpenUrl,
        ButtonType.PostText,
        ButtonType.ShowConfirmation
    ];

    sectionAlias(section: Section) {
        switch (section.SectionType) {
            case SectionType.Text:
                {
                    let ts = section as TextSection;
                    return ts.Text || ts.SectionType;
                }
            case SectionType.Image:
            case SectionType.Audio:
            case SectionType.Video:
            case SectionType.EmbeddedHtml:
            case SectionType.Gif:
            case SectionType.Graph:
            case SectionType.Carousel:
                {
                    let tcs = section as TitleCaptionSection;
                    return tcs.Title || tcs.Caption || tcs.SectionType;
                }
            default:
                return section.SectionType;
        }
    }
    chatButtonAlias(btn: Button) {
        return btn.ButtonName || btn.ButtonText || btn.ButtonType;
    }

    editorTypeFromSectionType(secType: SectionType): EditorType {
        switch (secType) {
            case SectionType.Text:
                return EditorType.Text;
            case SectionType.Image:
            case SectionType.Audio:
            case SectionType.Video:
            case SectionType.Gif:
            case SectionType.PrintOTP:
            case SectionType.EmbeddedHtml:
                return EditorType.TitleCaptionUrl;
            case SectionType.Carousel:
                return EditorType.Carousel;
            default:
                return EditorType.Text;
        }
    }
    chatButtonFieldVisible(btn: Button, fieldName: string) {
        var hidden = false;
        switch (btn.ButtonType) {
            case ButtonType.PostText:
                hidden = true; //Hide all. Probably!
                break;
            case ButtonType.OpenUrl:
                hidden = !(['Url'].indexOf(fieldName) != -1);//Show only Url field
                break;
            case ButtonType.GetText:
            case ButtonType.GetNumber:
            case ButtonType.GetAddress:
            case ButtonType.GetEmail:
            case ButtonType.GetPhoneNumber:
                //if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
                hidden = ['NextNodeId', 'DeepLinkUrl', 'Url', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case ButtonType.GetTime:
            case ButtonType.GetDate:
            case ButtonType.GetDateTime:
            case ButtonType.GetLocation:
                hidden = ['NextNodeId', 'DeepLinkUrl', 'Url', 'APIResponseMatchKey', 'APIResponseMatchValue', 'PostfixText', 'PrefixText'].indexOf(fieldName) != -1;
                break;
            case ButtonType.GetImage:
            case ButtonType.GetFile:
            case ButtonType.GetAudio:
            case ButtonType.GetVideo:
                //if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
                hidden = ['NextNodeId', 'DeepLinkUrl', 'PlaceholderText', 'Url', 'PostfixText', 'PrefixText', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case ButtonType.GetItemFromSource:
                hidden = ['NextNodeId', 'DeepLinkUrl', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case ButtonType.NextNode:
                hidden = ['NextNodeId', 'PostfixText', 'PrefixText', 'DeepLinkUrl', 'Url', 'PlaceholderText'].indexOf(fieldName) != -1;
                break;
            case ButtonType.DeepLink:
                hidden = ['NextNodeId', 'Url', 'PostfixText', 'PrefixText', 'PlaceholderText', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case ButtonType.GetAgent:
                hidden = true; //Hide all. Probably!
                break;
            case ButtonType.ShowConfirmation:
                hidden = true; //Hide all. Probably!
                break;
            case ButtonType.FetchChatFlow:
                hidden = ['DeepLinkUrl', 'PlaceholderText', 'PostfixText', 'PrefixText', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            default:
                break;
        }
        return hidden;
    }
    sectionIcon(section: Section) {
        switch (section.SectionType) {
            case SectionType.Image:
                return 'fa-picture-o';
            default:
                return 'fa-file-o';
        }
    }

    addSection(chatNode: ChatNode, sectionType: SectionType) {
        switch (sectionType) {
            default:
                chatNode.Sections.push({
                    SectionType: sectionType,
                    _id: new ObjectID().toHexString()
                });

                this.globalsService.chatFlowComponent.updateLayout();
                break;
        }
    }

    sectionMoveUp(chatNode: ChatNode, section: Section) {
        var current = chatNode.Sections.indexOf(section);
        if (current != -1)
            this.arrayMove(chatNode.Sections, current, current - 1);
    }
    sectionMoveDown(chatNode: ChatNode, section: Section) {
        var current = chatNode.Sections.indexOf(section);
        if (current != -1)
            this.arrayMove(chatNode.Sections, current, current + 1);
    }
    sectionDelete(chatNode: ChatNode, section: Section) {
        var current = chatNode.Sections.indexOf(section);
        if (current != -1) {
            if (confirm(`Delete section '${this.sectionAlias(section)}'`)) {
                chatNode.Sections.splice(current, 1);

                this.globalsService.chatFlowComponent.chatFlowNetwork.updateChatNodeConnections();
                this.globalsService.chatFlowComponent.updateLayout();
            }
        }
    }
    addButton(chatNode: ChatNode) {
        chatNode.Buttons.push({
            _id: new ObjectID().toHexString(),
            ButtonText: "New Button",
            ButtonType: ButtonType.NextNode
        });

        this.globalsService.chatFlowComponent.updateLayout();
    }

    buttonMoveUp(chatNode: ChatNode, btn: Button) {
        var current = chatNode.Buttons.indexOf(btn);
        if (current != -1)
            this.arrayMove(chatNode.Buttons, current, current - 1);
    }
    buttonMoveDown(chatNode: ChatNode, btn: Button) {
        var current = chatNode.Buttons.indexOf(btn);
        if (current != -1)
            this.arrayMove(chatNode.Buttons, current, current + 1);
    }
    buttonDelete(chatNode: ChatNode, btn: Button) {
        var current = chatNode.Buttons.indexOf(btn);
        if (current != -1) {
            if (confirm(`Delete button '${this.chatButtonAlias(btn)}'?`)) {
                chatNode.Buttons.splice(current, 1);

                this.globalsService.chatFlowComponent.chatFlowNetwork.updateChatNodeConnections();
                this.globalsService.chatFlowComponent.updateLayout();
            }
        }
    }
    nodeContentMenu(chatNodeVM: ChatNodeVM, event: MouseEvent, options: MdButton) {
        event.preventDefault();

        let btn = options._elementRef.nativeElement as HTMLButtonElement;
        btn.click();
    }

    resetOtherStartNodes(chatNode: ChatNode) {
        this.globalsService.chatFlowComponent.chatFlowNetwork.chatNodeVMs.forEach(vm => {
            if (vm.chatNode != chatNode)
                vm.chatNode.IsStartNode = false;
        });
    }

    test(chatNode: ChatNode) {
        alert(JSON.stringify(chatNode.Sections[chatNode.Sections.length - 1], null, 4));
    }

    arrayMove_RAW(array: any[], old_index, new_index) {
        if (new_index >= array.length) {
            var k = new_index - array.length;
            while ((k--) + 1) {
                array.push(undefined);
            }
        }
        array.splice(new_index, 0, array.splice(old_index, 1)[0]);
        return array; // for testing purposes
    }

    arrayMove(array: any[], old_index, new_index) {
        if (new_index >= array.length || new_index < 0)
            throw 'new index cannot be greater than or equal to array length or less than zero';

        array.splice(new_index, 0, array.splice(old_index, 1)[0]);
    }
}
