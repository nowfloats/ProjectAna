import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListComponent } from './list/list.component';

export const MANAGE_USERS_ROUTES: Routes = [
	{
		path: "",
		component: ListComponent
	}
];

@NgModule({
	declarations: [
		ListComponent
	]
})
export class ManageUsersModule { }

