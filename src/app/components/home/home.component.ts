import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { DataService } from '../../services/data.service';
import { InfoDialogService } from '../../services/info-dialog.service';
import { AnalyticsWindowService } from '../../services/analytics-window.service';
import { AnalyticsPickerParams, AnalyticsPickerComponent } from '../shared/analytics-picker/analytics-picker.component';
import { MatDialog } from '@angular/material';
@Component({
	selector: 'app-home',
	templateUrl: './home.component.html',
	styleUrls: ['./home.component.css']
})
export class HomeComponent {
	constructor(
		private router: Router,
		private loginService: LoginService,
		private infoDialog: InfoDialogService,
		private dialog: MatDialog,
		private dataService: DataService) {
	}

	studio() {
		this.router.navigateByUrl('/studio');
	}

	userManagement() {
		this.loginService.performLogin(true, "/", true, (done) => {
			if (!done) {
				this.infoDialog.alert("Login Required", "You must be logged in to your Ana chat server to manage users");
				return;
			}
			if (this.dataService.loggedInUser) {
				if (this.dataService.isSuperAdmin()) {
					this.router.navigateByUrl('/manage-users');
				} else if (this.dataService.isBizAdmin() && this.dataService.loggedInUser.businessId) {
					this.router.navigateByUrl('/manage-users/users?bizId=' + this.dataService.loggedInUser.businessId);
				} else {
					this.infoDialog.alert("Unauthorized!", "Only a super admin or a business admin can login into user management");
				}
			}
		});
	}

	analytics() {
		this.loginService.performLogin(true, null, true, (done) => {
			if (!done) {
				this.infoDialog.alert("Login Required", "You must be logged in to your Ana chat server to view analytics");
				return;
			}
			if (this.dataService.loggedInUser) {
				if (this.dataService.isSuperAdmin()) {
					this.openAnalyticsPicker();
				} else if ((this.dataService.isBizAdmin() || this.dataService.isFlowManager()) && this.dataService.loggedInUser.businessId) {
					this.openAnalyticsPicker({
						businessId: this.dataService.loggedInUser.businessId
					});
				} else {
					this.infoDialog.alert("Unauthorized!", "Only a super admin, a business admin or a flow manager can login into user management");
				}
			}
		});
	}

	openAnalyticsPicker(params?: AnalyticsPickerParams) {
		let d = this.dialog.open(AnalyticsPickerComponent, {
			width: 'auto',
			data: params
		});
	}
}