import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from "@angular/router";
import { DataService, BusinessAccount } from '../../../services/data.service';
import { MatDialog } from '@angular/material';
import { LoginComponent } from '../../shared/login/login.component';
@Component({
	selector: 'app-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements AfterViewInit {
	constructor(private dataService: DataService, private dialog: MatDialog, private route: Router) { }

	ngAfterViewInit(): void {
		Promise.resolve(true).then(() => {
			this.dataService.userLoggedinCheck((loggedin) => {
				if (!loggedin) {
					let d = this.dialog.open(LoginComponent, {
						width: '600px',
					});

					d.afterClosed().subscribe(x => {
						if (x == true) {
							this.loadAccounts();
						}
					});
				}
			})
		});
	}
	accounts: BusinessAccount[];

	loadAccounts() {
		this.dataService.getBusinessAccounts().subscribe(x => {
			if (x.success) {
				this.accounts = x.data.content;
			} else
				this.dataService.handleTypedError(x.error, "Unable to load business accounts", "Something went wrong while loading business accounts. Please try again.");
		}, err => {
			this.dataService.handleError(err, "Unable to load business accounts", "Something went wrong while loading business accounts. Please try again.");
		});
	}

	logout() {
		this.dataService.logout();
		this.route.navigateByUrl('/');
	}
}
