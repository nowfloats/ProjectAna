import { Component, OnInit } from '@angular/core';
import { Route, ActivatedRoute } from '@angular/router';
import { User } from '../../../models/data.models';
import { DataService } from '../../../services/data.service';
import { InfoDialogService } from '../../../services/info-dialog.service';
import { MatDialog } from '@angular/material';
import { CreateUserComponent } from '../../shared/create-user/create-user.component';

@Component({
	selector: 'app-users',
	templateUrl: './users.component.html',
	styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

	bizId: string;
	constructor(
		private route: ActivatedRoute,
		private infoDialog: InfoDialogService,
		private dialog: MatDialog,
		private dataService: DataService) {
		this.route.queryParamMap.subscribe(x => {
			let bizId = x.get('bizid');
			if (bizId) {
				this.bizId = bizId;
				this.loadUsers();
			}
		});
	}

	ngOnInit() {
	}

	createUserDialog() {
		this.dialog.open(CreateUserComponent, {
			width: '60%',
			data: this.bizId
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

	loadUsers() {
		if (this.bizId) {
			this.dataService.getUsers(this.bizId, this.page).subscribe(x => {
				if (x.success) {
					this.users = x.data.content;
				} else {
					this.dataService.handleTypedError(x.error, "Unable to load users", "Something went wrong while loading the users. Please try again.");
				}
			}, err => {
				this.dataService.handleError(err, "Unable to load users", "Something went wrong while loading the users. Please try again.");
			});
		}
	}

	updateUserPassword(user: User) {
		this.infoDialog.alert("Coming soon", "Check back in some time!");
	}
	userRole(user: User) {
		return user.roles.map(x => x.label).join(', ');
	}
}
