import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BizAccountsComponent } from './biz-accounts/biz-accounts.component';
import { SharedModule } from '../../shared.modul;
import { UsersComponent } from './users/users.component'e';
export const MANAGE_USERS_ROUTES: Routes = [
	{
		path: "",
		component: BizAccountsComponent
	}
];

@NgModule({
	declarations: [
		BizAccountsCompone,
		UsersComponentnt
	],
	imports: [
		SharedModule
	]
})
export class ManageUsersModule { }

