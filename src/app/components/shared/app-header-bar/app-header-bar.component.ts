import { Component, OnInit, AfterViewInit, Input } from '@angular/core';
import { Router } from "@angular/router";
import { DataService, BusinessAccount, LoginData, BusinessAccountStatus } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialog } from '@angular/material';
import { LoginComponent } from '../../shared/login/login.component';

@Component({
	selector: 'app-header-bar',
	templateUrl: './app-header-bar.component.html',
	styleUrls: ['./app-header-bar.component.css']
})
export class AppHeaderBarComponent implements OnInit, AfterViewInit {

	@Input('closePath')
	closePath: string = "/";

	@Input('skipAuth')
	skipAuth: boolean = false;

	@Input('title')
	title: string = "";

	constructor(private dataService: DataService, private dialog: MatDialog, private router: Router, private infoDialog: InfoDialogService) { }

	ngOnInit() {
	}

	ngAfterViewInit(): void {
		Promise.resolve(true).then(() => {
			this.dataService.userLoggedinCheck((loggedin) => {
				if (!loggedin && this.skipAuth == false) {
					let d = this.dialog.open(LoginComponent, {
						width: '600px',
					});

					d.afterClosed().subscribe(x => {
						if (x == true) {
							this.loggedInUser = this.dataService.loggedInUser;
							if (this.afterInit)
								this.afterInit();
						} else {
							this.router.navigateByUrl('/');
						}
					});
				} else {
					this.loggedInUser = this.dataService.loggedInUser;
					if (this.afterInit)
						this.afterInit();
				}
			})
		});
	}

	logout() {
		this.dataService.logout();
		this.router.navigateByUrl('/');
	}

	close() {
		this.router.navigateByUrl(this.closePath);
	}

	loggedInUser: LoginData;

	roles() {
		if (this.loggedInUser && this.loggedInUser.roles)
			return this.loggedInUser.roles.map(x => x.label).join(', ');
		return "";
	}

	afterInit: () => void;
}
