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
		this.analyticsWindow.open(this.dataService.getAnalyticsApiBase(), this.selectedBusinessAccount.id, this.selectedBusinessAccount.name);
	}
}

export interface AnalyticsPickerParams {
	businessId: string;
}
