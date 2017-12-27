import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Route, ActivatedRoute } from '@angular/router';
import { User, BusinessAccount } from '../../../models/data.models';
import { DataService } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialog } from '@angular/material';
import { CreateUserComponent, UserDialogParam, UserDialogMode } from '../../shared/create-user/create-user.component';
import { AppHeaderBarComponent } from '../../shared/app-header-bar/app-header-bar.component';
import { UpdatePasswordComponent } from '../../shared/update-password/update-password.component';

@Component({
	selector: 'app-users',
	templateUrl: './users.component.html',
	styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit, AfterViewInit {
	ngAfterViewInit(): void {
		this.appHeader.afterInit = () => {
			this.route.queryParamMap.subscribe(x => {
				let bizId = x.get('bizId');
				if (bizId) {
					this.bizId = bizId;
					this.loadUsers();
					this.loadBusinessDetails();
				}
			});
		};
	}

	@ViewChild(AppHeaderBarComponent)
	appHeader: AppHeaderBarComponent;

	businessAccount: BusinessAccount;
	bizId: string;
	constructor(
		private route: ActivatedRoute,
		private infoDialog: InfoDialogService,
		private dialog: MatDialog,
		private dataService: DataService) {
	}

	ngOnInit() {
	}

	loadBusinessDetails() {
		this.dataService.getBusinessDetails(this.bizId).subscribe(x => {
			this.businessAccount = x.data;
		}, err => {
			this.dataService.handleError(err, "Unable to load business details", "Something went wrong while trying to load business account details. Please try again.")
		});
	}

	createUserDialog() {
		let d =  this.dialog.open(CreateUserComponent, {
			width: '60%',
			data: <UserDialogParam>{
				bizId: this.bizId,
				mode: UserDialogMode.Create,
			}
		});
		d.afterClosed().subscribe(x => {
			this.loadUsers();
		});
	}

	users: User[] = [];
	page: number = 0;
	totalPages: number = 0;

	prevPage() {
		if (this.page > 0) {
			this.page--;
			this.loadUsers();
		}
	}
	nextPage() {
		if (this.page < this.totalPages) {
			this.page++;
			this.loadUsers();
		}
	}
	view(user: User) {
		this.dialog.open(CreateUserComponent, {
			width: '60%',
			data: <UserDialogParam>{
				mode: UserDialogMode.View,
				user: user
			}
		})
	}
	loadUsers() {
		if (this.bizId) {
			this.dataService.getUsers(this.bizId, this.page).subscribe(x => {
				//if (x.success) {
				this.users = x.content.filter(x => x.roles && x.roles.length > 0);
				this.totalPages = x.totalPages;
				//} else {
				//	debugger;
				//	this.dataService.handleTypedError(x.error, "Unable to load users", "Something went wrong while loading the users. Please try again.");
				//}
			}, err => {
				this.dataService.handleError(err, "Unable to load users", "Something went wrong while loading the users. Please try again.");
			});
		}
	}

	updateUserPassword(user: User) {
		this.dialog.open(UpdatePasswordComponent, {
			width: '40%',
			data: user
		});
	}

	userRole(user: User) {
		if (user.roles) {
			return user.roles.map(x => x.label).join(', ');
		}
		return "";
	}
}
