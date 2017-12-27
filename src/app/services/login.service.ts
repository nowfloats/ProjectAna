import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { MatDialog } from '@angular/material';
import { LoginComponent } from '../components/shared/login/login.component';
import { Router } from '@angular/router';

@Injectable()
export class LoginService {
	constructor(
		private dataService: DataService,
		private dialog: MatDialog,
		private router: Router) {
	}

	performLogin(skipAuth: boolean, fallbackUrl: string = "/", hardCheck: boolean = false, next?: () => void) {
		this.dataService.userLoggedinCheck((loggedin) => {
			if (!loggedin && skipAuth == false) {
				let d = this.dialog.open(LoginComponent, {
					width: '600px',
				});

				d.afterClosed().subscribe(x => {
					if (x == true) {
						if (next)
							next();
					} else
						this.router.navigateByUrl(fallbackUrl);
				});
			} else {
				if (next)
					next();
			}
		}, hardCheck);
	}

}