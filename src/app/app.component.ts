import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalsService } from './services/globals.service';
import { MdDialog, MdDialogRef, MD_DIALOG_DATA, MdSnackBar } from '@angular/material';
import { ChatServerManagerComponent } from './components/common/chat-server-manager/chat-server-manager.component';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.css']
})
export class AppComponent {
	constructor(
		public global: GlobalsService,
		public router: Router,
		public dialog: MdDialog) { }

	loading() {
		return this.global.loading;
	}
	hideLoading() {
		this.global.loading = false;
	}
	
	goto(path: string) {
		this.router.navigateByUrl(path);
	}
}
