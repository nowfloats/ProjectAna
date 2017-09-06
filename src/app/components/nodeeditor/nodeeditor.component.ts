import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { MdDialog, MdDialogRef, MD_DIALOG_DATA } from '@angular/material';
import 'rxjs/add/operator/filter';

import * as models from '../../models/chatflow.models';
import * as chatflow from '../../components/chatflow/chatflow.component';
import { ChatFlowService } from '../../services/chatflow.service';

@Component({
    selector: 'app-nodeeditor',
    templateUrl: './nodeeditor.component.html',
    styleUrls: ['./nodeeditor.component.css']
})
export class NodeEditorComponent implements OnInit {
    constructor(
       private chatFlowService: ChatFlowService,
       public dialogRef: MdDialogRef<NodeEditorComponent>,
       @Inject(MD_DIALOG_DATA) public chatNode: models.ChatNode) { }

    //constructor(private chatFlowService: ChatFlowService) { }
    /*
     chatNode: models.ChatNode = {
        ApiMethod: null,
        ApiResponseDataRoot: '',
        ApiUrl: '',
        Buttons: [],
        CardFooter: '',
        CardHeader: '',
        Emotion: '',
        GroupName: '',
        Id: '',
        Name: '',
        NextNodeId: '',
        NodeType: models.NodeType.Combination,
        Placement: null,
        RequiredVariables: [],
        Sections: [],
        TimeoutInMs: 0,
        VariableName: '',
        IsStartNode: false
     };
    */

    nodeTypes: models.NodeType[] = [
        models.NodeType.ApiCall,
        models.NodeType.Combination,
        models.NodeType.Card,
    ];
    apiMethods: models.APIMethod[] = [
        models.APIMethod.GET,
        models.APIMethod.POST,
    ];
    cardPlacements: models.CardPlacement[] = [
        models.CardPlacement.Center,
        models.CardPlacement.Incoming,
        models.CardPlacement.Outgoing,
    ];
    buttonTypes: models.ButtonType[] = [
        models.ButtonType.DeepLink,
        models.ButtonType.FetchChatFlow,
        models.ButtonType.GetAddress,
        models.ButtonType.GetAgent,
        models.ButtonType.GetAudio,
        models.ButtonType.GetDate,
        models.ButtonType.GetDateTime,
        models.ButtonType.GetEmail,
        models.ButtonType.GetImage,
        models.ButtonType.GetItemFromSource,
        models.ButtonType.GetLocation,
        models.ButtonType.GetNumber,
        models.ButtonType.GetPhoneNumber,
        models.ButtonType.GetText,
        models.ButtonType.GetTime,
        models.ButtonType.GetVideo,
        models.ButtonType.NextNode,
        models.ButtonType.OpenUrl,
        models.ButtonType.PostText,
        models.ButtonType.ShowConfirmation
    ];
    editorTypeFromSectionType(secType: models.SectionType): models.EditorType {
        switch (secType) {
            case models.SectionType.Text:
                return models.EditorType.Text;
            case models.SectionType.Image:
            case models.SectionType.Audio:
            case models.SectionType.Video:
            case models.SectionType.Gif:
            case models.SectionType.PrintOTP:
            case models.SectionType.EmbeddedHtml:
                return models.EditorType.TitleCaptionUrl;
            case models.SectionType.Carousel:
                return models.EditorType.Carousel;
            default:
                return models.EditorType.Text;
        }
    }

    titleCaptionUrlAlias(section: models.TitleCaptionUrlSection) {
        return section.Title || section.Caption || section.SectionType;
    }
    chatButtonAlias(btn: models.Button) {
        return btn.ButtonName || btn.ButtonText || btn.ButtonType;
    }
    chatButtonFieldVisible(btn: models.Button, fieldName: string) {
        var hidden = false;
        switch (btn.ButtonType) {
            case models.ButtonType.PostText:
                hidden = true; //Hide all. Probably!
                break;
            case models.ButtonType.OpenUrl:
                hidden = !(['Url'].indexOf(fieldName) != -1);//Show only Url field
                break;
            case models.ButtonType.GetText:
            case models.ButtonType.GetNumber:
            case models.ButtonType.GetAddress:
            case models.ButtonType.GetEmail:
            case models.ButtonType.GetPhoneNumber:
                //if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
                hidden = ['NextNodeId', 'DeepLinkUrl', 'Url', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case models.ButtonType.GetTime:
            case models.ButtonType.GetDate:
            case models.ButtonType.GetDateTime:
            case models.ButtonType.GetLocation:
                hidden = ['NextNodeId', 'DeepLinkUrl', 'Url', 'APIResponseMatchKey', 'APIResponseMatchValue', 'PostfixText', 'PrefixText'].indexOf(fieldName) != -1;
                break;
            case models.ButtonType.GetImage:
            case models.ButtonType.GetAudio:
            case models.ButtonType.GetVideo:
                //if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
                hidden = ['NextNodeId', 'DeepLinkUrl', 'PlaceholderText', 'Url', 'PostfixText', 'PrefixText', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case models.ButtonType.GetItemFromSource:
                hidden = ['NextNodeId', 'DeepLinkUrl', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case models.ButtonType.NextNode:
                hidden = ['NextNodeId', 'PostfixText', 'PrefixText', 'DeepLinkUrl', 'Url', 'PlaceholderText'].indexOf(fieldName) != -1;
                break;
            case models.ButtonType.DeepLink:
                hidden = ['NextNodeId', 'Url', 'PostfixText', 'PrefixText', 'PlaceholderText', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            case models.ButtonType.GetAgent:
                hidden = true; //Hide all. Probably!
                break;
            case models.ButtonType.ShowConfirmation:
                hidden = true; //Hide all. Probably!
                break;
            case models.ButtonType.FetchChatFlow:
                hidden = ['DeepLinkUrl', 'PlaceholderText', 'PostfixText', 'PrefixText', 'APIResponseMatchKey', 'APIResponseMatchValue'].indexOf(fieldName) != -1;
                break;
            default:
                break;
        }
        return hidden;
    }
    sectionIcon(section: models.Section) {
        switch (section.SectionType) {
            case models.SectionType.Image:
                return 'fa-picture-o';
            default:
                return 'fa-file-o';
        }
    }

    ngOnInit(): void {
        //this.chatFlowService.fetchChatFlowPack('599c3caa460b5053e4b09869').subscribe(x => {
        //    this.chatNode = x.ChatNodes[0];
        //    console.log(this.chatNode);
        //}, err => console.error(err));
    }
}
