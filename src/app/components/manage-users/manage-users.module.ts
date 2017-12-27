import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BizAccountsComponent } from './biz-accounts/biz-accounts.component';
import { SharedModule } from '../../shared.module';
import { UsersComponent } from './users/users.component';
export const MANAGE_USERS_ROUTES: Routes = [
	{
		path: "",
		component: BizAccountsComponent
	},
	{
		path: "users",
		component: UsersComponent
	}
];

@NgModule({
	declarations: [
		BizAccountsComponent,
		UsersComponent
	],
	imports: [
		SharedModule
	]
})
export class ManageUsersModule { }

