import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from "@angular/router";
import { DataService, BusinessAccount, LoginData, BusinessAccountStatus } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialog } from '@angular/material';
import { LoginComponent } from '../../shared/login/login.component';
import { EditBusinessAccountComponent } from '../../shared/edit-business-account/edit-business-account.component';
@Component({
	selector: 'app-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.css']
})
export class ListComponent implements AfterViewInit {
	constructor(private dataService: DataService, private dialog: MatDialog, private route: Router, private infoDialog: InfoDialogService) { }

	ngAfterViewInit(): void {
		Promise.resolve(true).then(() => {
			this.dataService.userLoggedinCheck((loggedin) => {
				if (!loggedin) {
					let d = this.dialog.open(LoginComponent, {
						width: '600px',
					});

					d.afterClosed().subscribe(x => {
						if (x == true) {
							this.loggedInUser = this.dataService.loggedInUser;
							this.loadAccounts();
						}
					});
				} else {
					this.loggedInUser = this.dataService.loggedInUser;
					this.loadAccounts();
				}
			})
		});
	}

	logout() {
		this.dataService.logout();
		this.route.navigateByUrl('/');
	}
	loggedInUser: LoginData;


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

	editBusinessAccount(data?: BusinessAccount) {
		let d = this.dialog.open(EditBusinessAccountComponent, {
			width: '40%',
			data: data
		});
		d.afterClosed().subscribe(x => {
			this.loadAccounts();
		});
	}
	BusinessAccountStatus = BusinessAccountStatus;
	updateBusinessAccountStatus(account: BusinessAccount, status: BusinessAccountStatus) {
		let work = (status == BusinessAccountStatus.ACTIVE ? "activate" : "deactivate");
		this.infoDialog.confirm("Confirmation", `Are you sure you want to ${work} the business account?`, (ok) => {
			if (ok) {
				this.dataService.updateBusinessAccountStatus(account, status).subscribe(x => {
					if (x.success) {
						this.infoDialog.alert("Done", "Business account status updated");
						this.loadAccounts();
					} else {
						this.dataService.handleTypedError(x.error, "Unable to update business account status", "Something went wrong while updating the business account. Please try again.");
					}
				}, err => {
					this.dataService.handleError(err, "Unable to update business account status", "Something went wrong while updating the business account. Please try again.");
				});
			}
		});
	}
}
