import { Component, OnInit, AfterViewInit, Input } from '@angular/core';
import { Router } from "@angular/router";
import { DataService } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialog } from '@angular/material';
import { LoginComponent } from '../../shared/login/login.component';
import { LoginData } from '../../../models/data.models';
import { LoginService } from '../../../services/login.service';

@Component({
	selector: 'app-header-bar',
	templateUrl: './app-header-bar.component.html',
	styleUrls: ['./app-header-bar.component.css']
})
export class AppHeaderBarComponent implements OnInit, AfterViewInit {

	@Input('backPath')
	backPath: string = "/";

	@Input('skipAuth')
	skipAuth: boolean = false;

	@Input('hideBackButton')
	hideBackButton: boolean = false;

	@Input('title')
	title: string = "";

	constructor(
		private dataService: DataService,
		private dialog: MatDialog,
		private router: Router,
		private infoDialog: InfoDialogService,
		private loginService: LoginService) { }

	ngOnInit() {

	}

	ngAfterViewInit(): void {
		this.checkAndUpdate();
	}

	checkAndUpdate() {
		Promise.resolve(true).then(() => {
			this.loginService.performLogin(this.skipAuth, "/", () => {
				this.loggedInUser = this.dataService.loggedInUser;
				if (this.afterInit)
					this.afterInit();
			});
		});
	}
	logout() {
		this.dataService.logout();
		this.router.navigateByUrl('/');
	}

	back() {
		this.router.navigateByUrl(this.backPath);
	}

	loggedInUser: LoginData;

	roles() {
		if (this.loggedInUser && this.loggedInUser.roles)
			return this.loggedInUser.roles.map(x => x.label).join(', ');
		return "";
	}

	afterInit: () => void;
}
