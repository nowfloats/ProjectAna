import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { DataService } from '../../services/data.service';
import { InfoDialogService } from '../../services/info-dialog.service';
import { AnalyticsWindowService } from '../../services/analytics-window.service';
import { AnalyticsPickerParams, AnalyticsPickerComponent } from '../shared/analytics-picker/analytics-picker.component';

import { MatDialog } from '@angular/material';
import { BusinessPickerComponent, BusinessPickerParam, ChoosenBizAccChatProj } from '../shared/business-picker/business-picker.component';
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
					this.openAnalyticsPicker({ askFlowId: true });
				} else if ((this.dataService.isBizAdmin() || this.dataService.isFlowManager()) && this.dataService.loggedInUser.businessId) {
					this.openAnalyticsPicker({
						askFlowId: true,
						businessId: this.dataService.loggedInUser.businessId
					});
				} else {
					this.infoDialog.alert("Unauthorized!", "Only a super admin, a business admin or a flow manager can login into user management");
				}
			}
		});
	}

	openAnalyticsPicker(params: BusinessPickerParam) {
		let d = this.dialog.open(BusinessPickerComponent, {
			width: 'auto',
			data: params
		});

		d.afterClosed().subscribe((x: ChoosenBizAccChatProj) => {
			if (x && x.bizAccount && x.chatProj) {
				this.infoDialog.prompt("Analytics Server Url", "Please enter the analytics server url", (result) => {
					if (result) {
						localStorage.setItem('analyticsApiBase', result);
						let url = `/analytics?apiBase=${result}&businessId=${x.bizAccount.id}&businessName=${x.bizAccount.name}&chatFlowId=${x.chatProj.id}`;
						this.router.navigateByUrl(url);
					}
				}, localStorage.getItem('analyticsApiBase'));
			}
		});
	}
}