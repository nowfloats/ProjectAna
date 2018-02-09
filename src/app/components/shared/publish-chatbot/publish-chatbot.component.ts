import { Component, OnInit, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatAutocompleteSelectedEvent } from '@angular/material';
import { ChatBotReferance, ChatServerConnection } from '../../../models/app.models';
import { SettingsService } from '../../../services/settings.service';
import { ChatFlowService } from '../../../services/chatflow.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { ChatServerManagerComponent } from '../chat-server-manager/chat-server-manager.component';
import * as models from '../../../models/chatflow.models';
import { DataService } from '../../../services/data.service';
import { LoginService } from '../../../services/login.service';
import { ChatProject, BusinessAccount } from '../../../models/data.models';
import { GlobalsService } from '../../../services/globals.service';
import { CreateChatbotComponent, BusinessDetails } from '../create-chatbot/create-chatbot.component';

import { Observable } from 'rxjs/Observable';
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';
import { FormControl } from '@angular/forms';
import { BusinessPickerComponent } from '../business-picker/business-picker.component';

@Component({
	selector: 'app-publish-chatbot',
	templateUrl: './publish-chatbot.component.html',
	styleUrls: ['./publish-chatbot.component.css']
})
export class PublishChatbotComponent implements OnInit {
	constructor(
		private settings: SettingsService,
		private globals: GlobalsService,
		private dataService: DataService,
		private loginService: LoginService,
		private dialog: MatDialog,
		private infoDialog: InfoDialogService,
		private dialogRef: MatDialogRef<PublishChatbotComponent>,
		@Inject(MAT_DIALOG_DATA) private pack: models.ChatFlowPack) {
	}

	chatProjects: ChatProject[] = [];
	businessId: string;
	ngOnInit() {
		Promise.resolve(null).then(() => {
			this.init();
		});
	}

	init() {
		this.infoDialog.showSpinner();
		this.loginService.performLogin(false, null, true, (done) => {
			this.infoDialog.hideSpinner();

			if (this.dataService.loggedInUser) {
				if (this.dataService.isBizAdmin() || this.dataService.isFlowManager()) {
					this.businessId = this.dataService.loggedInUser.businessId;
					this.loadChatProjects();
				} else {
					let d = this.dialog.open(BusinessPickerComponent, {
						width: "auto",
						data: null
					});
					d.afterClosed().subscribe(x => {
						if (x) {
							let ba = x as BusinessAccount;
							this.businessId = ba.id;
							this.loadChatProjects();
						} else {
							this.dialogRef.close();
						}
					});
					//this.infoDialog.alert("Unauthorized!", "Only a business admin or a flow manager can publish a flow", () => {
					//	this.dialogRef.close();
					//});
				}
			} else {
				this.dialogRef.close();
			}
		});
	}

	createNewChatProject() {
		let d = this.dialog.open(CreateChatbotComponent, {
			width: 'auto',
			disableClose: true,
			data: <BusinessDetails>{
				id: this.businessId
			}
		});
		d.afterClosed().subscribe(x => {
			if (x) {
				this.loadChatProjects();
			}
		});
	}

	loadChatProjects() {
		let bizId = this.businessId;
		this.infoDialog.showSpinner();
		this.dataService.getChatProjects(bizId, 0, 10000).subscribe(x => {
			this.infoDialog.hideSpinner();
			if (x.success) {
				this.chatProjects = x.data.content;
			} else {
				this.dataService.handleTypedError(x.error, "Unable to fetch chat projects", "Something went wrong while trying to fetch chat projects. Please try again.");
			}
		}, err => {
			this.infoDialog.hideSpinner();
			this.dataService.handleError(err, "Unable to fetch chat projects", "Something went wrong while trying to fetch chat projects. Please try again.");
		});
	}

	doPublish() {
		if (!this.selectedProject) {
			return;
		}
		this.infoDialog.showSpinner();
		this.selectedProject.source = this.pack;
		this.selectedProject.flow = this.globals.normalizeChatNodes(this.pack.ChatNodes);
		this.dataService.saveChatProject(this.selectedProject).subscribe(x => {
			this.infoDialog.hideSpinner();
			if (x.success) {
				this.infoDialog.alert('Done', 'Chatbot published successfully', () => this.dismiss());
			} else {
				this.dataService.handleTypedError(x.error, "Oops!", "Something went wrong while publishing the chat project! Please try again.");
			}
		}, err => {
			this.infoDialog.hideSpinner();
			this.dataService.handleError(err, "Oops!", "Something went wrong while publishing the chat project! Please try again.");
		});
	}

	dismiss() {
		this.dialogRef.close();
	}
	selectedProject: ChatProject;
	optionSelected(event: MatAutocompleteSelectedEvent) {
		if (event.option && event.option.value) {
			this.selectedProject = event.option.value;
		}
	}
	displayWith(value: any) {
		return value ? value.name : null;
	}
	filter: string | ChatProject = "";
	filteredOptions() {
		if (typeof this.filter === 'string') {
			if (this.filter) {
				return this.chatProjects.filter(x => x.name.toLowerCase().indexOf((<string>this.filter).toLowerCase()) != -1);
			}
			return this.chatProjects;
		}
	}

}
