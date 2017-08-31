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
    // constructor(
    //   public dialogRef: MdDialogRef<NodeEditorComponent>,
    //   @Inject(MD_DIALOG_DATA) public chatNode: chatflow.ChatNodeVM) { }


    constructor(private chatFlowService: ChatFlowService) { }
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
    ngOnInit(): void {
        this.chatFlowService.fetchChatFlowPack('599c3caa460b5053e4b09869').subscribe(x => {
            this.chatNode = x.ChatNodes[0];
            console.log(this.chatNode);
        }, err => console.error(err));
    }
}
