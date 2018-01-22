import { Component, OnInit, Inject, Optional } from '@angular/core';
import { DataService } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BusinessAccount } from '../../../models/data.models';
import { AnalyticsWindowService } from '../../../services/analytics-window.service';

@Component({
	selector: 'app-analytics-picker',
	templateUrl: './analytics-picker.component.html',
	styleUrls: ['./analytics-picker.component.css']
})
export class AnalyticsPickerComponent implements OnInit {

	constructor(
		private dataService: DataService,
		private infoDialog: InfoDialogService,
		private analyticsWindow: AnalyticsWindowService, 
		private dialogRef: MatDialogRef<AnalyticsPickerComponent>,
		@Optional() @Inject(MAT_DIALOG_DATA) private param: AnalyticsPickerParams) {
		if (param && param.businessId) {
			this.businessAccountReadonly = true;
		}
	}

	ngOnInit() {
		Promise.resolve(null).then(() => {
			this.init();
		})
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

	businessAccountReadonly: boolean = false;
	selectedBusinessAccount: BusinessAccount;
	businessAccounts: BusinessAccount[] = [];

	openAnalytics() {
		this.dialogRef.close();
		this.infoDialog.prompt("Analytics Server Url", "Please enter the analytics server url", (result) => {
			if (result) {
				localStorage.setItem('analyticsApiBase', result);
				this.analyticsWindow.open(result, this.selectedBusinessAccount.id, this.selectedBusinessAccount.name);
			}
		}, localStorage.getItem('analyticsApiBase'));
	}
}

export interface AnalyticsPickerParams {
	businessId: string;
}
