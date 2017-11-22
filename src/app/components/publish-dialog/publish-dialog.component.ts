import { Component, OnInit, Inject } from '@angular/core';
import { MdDialog, MdDialogRef, MD_DIALOG_DATA } from '@angular/material';
import { ChatBotProject, ChatBotReferance, ChatServerConnection } from '../../models/app.models';
import { SettingsService } from '../../services/settings.service';
import { ChatFlowService } from '../../services/chatflow.service';
import { ChatServerManagerComponent } from '../../components/chat-server-manager/chat-server-manager.component';
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
		private dialog: MdDialog,
		private dialogRef: MdDialogRef<PublishDialogComponent>,
		@Inject(MD_DIALOG_DATA) private pack: models.ChatFlowPack) {
		this.loadSavedConns();
	}

	loadSavedConns() {
		this.savedConns = this.settings.loadSavedConnections();
		this.selectedServer = null;
		this.selectedProject = null;
	}

	savedConns: ChatServerConnection[] = [];
	chatProjects: ChatBotProject[] = [];
	selectedServer: ChatServerConnection;
	selectedProject: ChatBotProject;

	ngOnInit() {
	}

	publish() {
		this.chatFlowService.chatProjectExists(this.selectedServer, this.selectedProject).subscribe(x => {
			if (confirm(`Chat project with id '${this.selectedProject.Id}' already exists. Publishing this will overwrite it. Do you want to proceed?`))
				this.doPublish();
		}, err => {
			this.doPublish();
		});
	}

	private doPublish() {
		this.chatFlowService.publishProject(this.selectedServer, this.selectedProject, this.pack).subscribe(x => {
			alert('Chatbot published successfully');
			this.dismiss();
		}, err => {
			alert('Oops! Something went wrong while publishing the chat project! Please try again.');
		});
	}

	managePublishServers() {
		let dialogRef = this.dialog.open(ChatServerManagerComponent, {
			width: '60%',
		});

		dialogRef.afterClosed().subscribe(x => {
			this.loadSavedConns();
		});
	}
	dismiss() {
		this.dialogRef.close();
	}
}
