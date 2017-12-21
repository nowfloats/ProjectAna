import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router  } from "@angular/router";
import { DataService } from '../../../services/data.service';
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
							this.loadUsers();
						}
					});
				}
			})
		});
	}

	loadUsers() {

	}

	logout() {
		this.dataService.logout();
		this.route.navigateByUrl('/');
	}
}
