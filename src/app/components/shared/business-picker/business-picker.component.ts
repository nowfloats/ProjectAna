import { Component, OnInit, Inject, Optional } from '@angular/core';
import { DataService } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BusinessAccount } from '../../../models/data.models';
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
		@Optional() @Inject(MAT_DIALOG_DATA) private businessId: string) {
		if (businessId) {
			this.businessAccountReadonly = true;
		}
	}

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
				if (this.businessId && this.businessAccounts) {
					let x = this.businessAccounts.filter(x => x.id == this.businessId);
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

	submit() {
		this.dialogRef.close(this.selectedBusinessAccount);
	}
}
