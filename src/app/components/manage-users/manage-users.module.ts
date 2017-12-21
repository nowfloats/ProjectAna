import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListComponent } from './list/list.component';
import { SharedModule } from '../../shared.module';
export const MANAGE_USERS_ROUTES: Routes = [
	{
		path: "",
		component: ListComponent
	}
];

@NgModule({
	declarations: [
		ListComponent
	],
	imports: [
		SharedModule
	]
})
export class ManageUsersModule { }

