import { Component, OnInit, Inject, Optional } from '@angular/core';
import { DataService } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BusinessAccount, ChatProject } from '../../../models/data.models';
import { AnalyticsWindowService } from '../../../services/analytics-window.service';
import { Router } from '@angular/router';

@Component({
	selector: 'app-business-picker',
	templateUrl: './business-picker.component.html',
	styleUrls: ['./business-picker.component.css']
})
export class BusinessPickerComponent implements OnInit {

	constructor(
		private dataService: DataService,
		private infoDialog: InfoDialogService,
		private router: Router,
		private analyticsWindow: AnalyticsWindowService,
		private dialogRef: MatDialogRef<BusinessPickerComponent>,
		@Inject(MAT_DIALOG_DATA) private param: BusinessPickerParam) {
		if (this.param.askFlowId) {
			this.title = "Choose business account and chat project";
		} else {
			this.title = "Choose business account";
		}
	}

	title = "";

	ngOnInit() {
		Promise.resolve(null).then(() => {
			this.init();
		});
	}

	init() {
		this.infoDialog.showSpinner();
		this.dataService.getBusinessAccounts(0, 10000).subscribe(x => {
			this.infoDialog.hideSpinner();
			if (x.success) {
				this.businessAccounts = x.data.content;
				if (this.param && this.param.businessId && this.businessAccounts) {
					let x = this.businessAccounts.filter(x => x.id == this.param.businessId);
					if (x && x.length > 0)
						this.selectedBusinessAccount = x[0];
				}
			} else {
				this.dataService.handleTypedError(x.error, "Unable to load business accounts", "Something went wrong while loading business account. Please try again.");
			}
		}, err => {
			this.infoDialog.hideSpinner();
			this.dataService.handleError(err, "Unable to load business accounts", "Something went wrong while loading business account. Please try again.");
		});
	}

	selectedBusinessAccount: BusinessAccount;
	businessAccounts: BusinessAccount[] = [];

	chatProjects: ChatProject[] = [];
	selectedChatProject: ChatProject;

	loadChatProjects() {
		if (!this.param.askFlowId)
			return false;
		if (!this.selectedBusinessAccount)
			return false;

		this.infoDialog.showSpinner();
		this.dataService.getChatProjects(this.selectedBusinessAccount.id, 0, 10000).subscribe(x => {
			this.infoDialog.hideSpinner();
			if (x.success) {
				this.chatProjects = x.data.content;
			}
			else {
				this.dataService.handleTypedError(x.error, "Unable to fetch chat projects", "Something went wrong while trying to fetch chat projects. Please try again.");
			}
		}, err => {
			this.infoDialog.hideSpinner();
			this.dataService.handleError(err, "Unable to fetch chat projects", "Something went wrong while trying to fetch chat projects. Please try again.");
		});
	}

	submit() {
		this.dialogRef.close(<ChoosenBizAccChatProj>{
			bizAccount: this.selectedBusinessAccount,
			chatProj: this.selectedChatProject
		});
	}

	valid() {
		if (this.param.askFlowId) {
			return this.selectedBusinessAccount && this.selectedChatProject;
		} else {
			return this.selectedBusinessAccount;
		}
	}
}

export interface ChoosenBizAccChatProj {
	bizAccount: BusinessAccount;
	chatProj: ChatProject;
}

export interface BusinessPickerParam {
	askFlowId: boolean;
	businessId?: string;
	flowId?: string;
}