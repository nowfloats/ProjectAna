import { Component, OnInit, Inject, Optional } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ChatBotReferance, ChatServerConnection } from '../../../models/app.models';
import { SettingsService } from '../../../services/settings.service';
import { ChatFlowService } from '../../../services/chatflow.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { ChatServerManagerComponent } from '../chat-server-manager/chat-server-manager.component';
import * as models from '../../../models/chatflow.models';
import { DataService, LoginData, BusinessAccount } from "../../../services/data.service";
import { GlobalsService } from '../../../services/globals.service';
import { Router } from "@angular/router";
import { FormControl, Validators } from '@angular/forms';

@Component({
	selector: 'app-edit-business-account',
	templateUrl: './edit-business-account.component.html',
	styleUrls: ['./edit-business-account.component.css']
})
export class EditBusinessAccountComponent implements OnInit {
	title: string = "Create business account";
	constructor(
		private global: GlobalsService,
		private infoDialog: InfoDialogService,
		private dataService: DataService,
		private dialogRef: MatDialogRef<EditBusinessAccountComponent>,

		@Optional()
		@Inject(MAT_DIALOG_DATA)
		public data: BusinessAccount) {
		this.dialogRef.disableClose = true;

		if (data) {
			this.account = data;
			this.title = "Edit business account";
		}
	}

	ngOnInit(): void {

	}

	account: BusinessAccount = {
		colors: [],
		email: "",
		logoUrl: "",
		name: "",
		phone: "",
	};

	primaryBGColor: string = "#8cc83c";
	primaryFGColor: string = "white";
	secondaryColor: string = "#3c3c3c";

	emailValid(val: string) {
		let r = this.global.emailValid(val);
		return r;
	}
	phoneValid(val: string) {
		return this.global.phoneValid(val);
	}
	pwdMatch(p1, p2) {
		return this.global.pwdMatch(p1, p2);
	}

	confirmPassword: string;
	save() {
		if (!this.global.emailValid(this.account.email) ||
			!this.global.phoneValid(this.account.phone)) {
			this.infoDialog.alert("Invalid details", "Please enter valid information in the fields provided");
			return;
		}
		//if (this.account.password) {
		//	if (!this.global.pwdMatch(this.account.password, this.confirmPassword)) {
		//		this.infoDialog.alert("Invalid details", "Password and confirm password should match. Password must be at least 6 characters");
		//		return;
		//	}
		//}
		
		this.account.colors = [
			{
				name: "PRIMARY_BG",
				value: this.primaryBGColor
			},
			{
				name: "PRIMARY_FG",
				value: this.primaryFGColor
			},
			{
				name: "SECONDARY",
				value: this.secondaryColor
			}
		];

		this.dataService.saveBusinessAccount(this.account).subscribe(x => {
			if (x.success) {
				this.infoDialog.alert("Done", "Business account has been saved successfully", () => {
					this.dialogRef.close();
				});
			} else {
				this.dataService.handleTypedError(x.error, "Unable to save business account", "Something went wrong while trying to save business account details");
			}
		}, err => {
			this.dataService.handleError(err, "Unable to save business account", "Something went wrong while trying to save business account details");
		});
	}
}