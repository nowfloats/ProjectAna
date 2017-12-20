import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { MdDialog, MdDialogRef, MD_DIALOG_DATA } from '@angular/material';
import 'rxjs/add/operator/filter';

import * as models from '../../../models/chatflow.models';
import * as chatflow from '../../../components/studio/chatflow/chatflow.component';
import { ChatFlowService } from '../../../services/chatflow.service';
import { GlobalsService } from '../../../services/globals.service';
import { InfoDialogService } from '../../../services/info-dialog.service';

@Component({
	selector: 'app-nodeeditor',
	templateUrl: './nodeeditor.component.html',
	styleUrls: ['./nodeeditor.component.css']
})
export class NodeEditorComponent {
	constructor(
		private chatFlowService: ChatFlowService,
		private infoDialog: InfoDialogService,
		public dialogRef: MdDialogRef<NodeEditorComponent>,
		@Inject(MD_DIALOG_DATA) public chatNode: models.ChatNode,
		public globalsService: GlobalsService) {

		this.MH = new models.ModelHelpers(globalsService, infoDialog);
	}
	SectionType = models.SectionType;
	dismiss() {
		this.dialogRef.close();
	}
	//Model Helpers
	MH: models.ModelHelpers;
}
