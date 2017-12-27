import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { DataService } from '../../services/data.service';
import { InfoDialogService } from '../../services/info-dialog.service';
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
		private dataService: DataService) { }

	studio() {
		this.router.navigateByUrl('/studio');
	}

	userManagement() {
		this.loginService.performLogin(false, "/", () => {
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
}