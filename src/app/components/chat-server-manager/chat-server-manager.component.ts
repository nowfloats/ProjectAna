import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MdDialogRef } from '@angular/material';
import { ChatServerConnection, ChatBotProject } from '../../models/app.models';
import { SettingsService } from '../../services/settings.service';
import { ChatFlowService } from '../../services/chatflow.service';
import { GlobalsService } from '../../services/globals.service';
import { MdSnackBar } from '@angular/material';

@Component({
	selector: 'app-chat-server-manager',
	templateUrl: './chat-server-manager.component.html',
	styleUrls: ['./chat-server-manager.component.css']
})
export class ChatServerManagerComponent implements OnInit {
	constructor(
		public settings: SettingsService,
		public chatFlowService: ChatFlowService,
		public global: GlobalsService,
		public snakbar: MdSnackBar,
		public router: Router,
		public dialogRef: MdDialogRef<ChatServerManagerComponent>) {
		this.savedConnections = this.settings.loadSavedConnections();
	}

	savedConnections: ChatServerConnection[] = [];

	ngOnInit(): void {
	}

	connectionAlias(conn: ChatServerConnection) {
		return conn.Name || conn.ServerUrl || 'New Publish Server';
	}

	deleteConnection(conn: ChatServerConnection) {
		var current = this.savedConnections.indexOf(conn);
		if (current != -1) {
			if (confirm(`Delete chat server connection '${this.connectionAlias(conn)}'`)) {
				this.savedConnections.splice(current, 1);
				this.saveConnections();
			}
		}
	}

	addChatProjectToConn(conn: ChatServerConnection) {
		if (!conn.ChatProjects)
			conn.ChatProjects = [];
		conn.ChatProjects.push({
			CreatedOn: new Date(),
			Id: '',
			Name: 'New Chat Project',
			UpdatedOn: new Date()
		});
	}

	deleteProject(conn: ChatServerConnection, proj: ChatBotProject) {
		var current = conn.ChatProjects.indexOf(proj);
		if (current != -1) {
			if (confirm(`Delete chat project '${proj.Name}'`)) {
				conn.ChatProjects.splice(current, 1);
				this.saveConnections();
			}
		}
	}

	saveConnections() {
		this.settings.saveSavedConnections(this.savedConnections);

		this.snakbar.open('Publish servers saved!', 'Dismiss', {
			duration: 3000
		});
		this.dialogRef.close();
	}

	addConnection() {
		let newConn: ChatServerConnection = {
			APIKey: '',
			APISecret: '',
			IsDefault: false,
			Name: '',
			ServerUrl: '',
			ChatProjects: []
		};
		this.savedConnections.push(newConn);
	}
}