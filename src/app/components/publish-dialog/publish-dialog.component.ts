import { Component, OnInit, Inject } from '@angular/core';
import { MdDialog, MdDialogRef, MD_DIALOG_DATA } from '@angular/material';
import { ChatBotProject, ChatBotReferance, ChatServerConnection } from '../../models/app.models';
import { SettingsService } from '../../services/settings.service';
import { ChatFlowService } from '../../services/chatflow.service';
import * as models from '../../models/chatflow.models';
@Component({
	selector: 'app-publish-dialog',
	templateUrl: './publish-dialog.component.html',
	styleUrls: ['./publish-dialog.component.css']
})
export class PublishDialogComponent implements OnInit {

	constructor(
		private settings: SettingsService,
		private chatFlowService: ChatFlowService,
		private dialogRef: MdDialogRef<PublishDialogComponent>,
		@Inject(MD_DIALOG_DATA) private pack: models.ChatFlowPack) {
		this.savedConns = this.settings.loadSavedConnections();
	}

	savedConns: ChatServerConnection[] = [];
	chatProjects: ChatBotProject[] = [];
	selectedServer: ChatServerConnection;
	selectedProject: ChatBotProject;

	ngOnInit() {
	}

	publish() {
		this.chatFlowService.publishProject(this.selectedServer, this.selectedProject, this.pack).subscribe(x => {
			alert(JSON.stringify(x));
		}, err => {
			alert(JSON.stringify(err.data));
		});
	}

	dismiss() {
		this.dialogRef.close();
	}
}
