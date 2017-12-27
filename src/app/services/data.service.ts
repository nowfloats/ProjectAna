import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { InfoDialogService } from './info-dialog.service';
import { ChatServerConnection, ChatBotProject } from '../models/app.models';
import { LoginData, APIResponse, ListContent, BusinessAccount, BusinessAccountStatus, ErrorItem, Role, ListData, UserRegisterModel, User } from '../models/data.models';

@Injectable()
export class DataService {

	constructor(private http: HttpClient, private infoDialog: InfoDialogService) {
		let connJSON = localStorage.getItem("conn");
		if (connJSON)
			this.conn = JSON.parse(connJSON);
	}
	private conn: ChatServerConnection;
	loggedInUser: LoginData;

	private normalizeBaseUrl(baseUrl: string) {
		baseUrl = baseUrl.replace(/\\$/, '');//Remove ending \ char if any
		if (!baseUrl.endsWith('/'))
			baseUrl += '/';
		return baseUrl;
	}

	private getHeaders() {
		if (this.loggedInUser && this.loggedInUser.accessToken)
			return new HttpHeaders({
				"access-token": this.loggedInUser.accessToken
			});
		return new HttpHeaders();
	}

	setConnection(conn: ChatServerConnection) {
		if (conn && conn.ServerUrl)
			conn.ServerUrl = this.normalizeBaseUrl(conn.ServerUrl);
		localStorage.setItem("conn", JSON.stringify(conn));
		this.conn = conn;
	}

	getRoles() {
		let h = this.getHeaders();
		return this.http.get(`${this.conn.ServerUrl}auth/roles`, { headers: h })
			.map(x => x as APIResponse<Role[]>);
	}

	getBusinessAccounts(page: number = 0, size: number = 10) {
		let h = this.getHeaders();
		return this.http.get(`${this.conn.ServerUrl}business/accounts?page=${page}&size=${size}`, { headers: h })
			.map(x => x as APIResponse<ListContent<BusinessAccount>>);
	}

	updateBusinessAccountStatus(account: BusinessAccount, status: BusinessAccountStatus) {
		let h = this.getHeaders();
		return this.http.put(this.conn.ServerUrl + "business/accounts/" + account.id + "/status/" + BusinessAccountStatus[<number>status], { headers: h })
			.map(x => x as APIResponse<BusinessAccount>);
	}
	saveBusinessAccount(account: BusinessAccount) {
		if (!account.id) {
			return this.http.post(this.conn.ServerUrl + "business/accounts", account,
				{ headers: this.getHeaders() }).map(x => x as APIResponse<BusinessAccount>);
		} else {
			return this.http.put(this.conn.ServerUrl + "business/accounts/" + account.id, account,
				{ headers: this.getHeaders() }).map(x => x as APIResponse<BusinessAccount>);
		}
	}

	getUsers(bizid: string, page: number = 0, size: number = 10) {
		let h = this.getHeaders();
		return this.http.get(`${this.conn.ServerUrl}auth/users?page=${page}&size=${size}`, { headers: h })
			.map(x => x as ListContent<User>);
	}

	createUser(user: UserRegisterModel) {
		let h = this.getHeaders();
		return this.http.get(`${this.conn.ServerUrl}auth/users/accounts/register`, { headers: h })
			.map(x => x as APIResponse<User>);
	}

	login(username: string, password: string) {
		return this.http.post(this.conn.ServerUrl + "auth/login", {
			"username": username,
			"password": password
		}).map(x => x as APIResponse<LoginData>);
	}

	checkLogin(data: LoginData) {
		return this.http.get(this.conn.ServerUrl + "auth/me", {
			headers: { "access-token": data.accessToken }
		}).map(x => x as APIResponse<LoginData>);
	}

	logout() {
		localStorage.removeItem("user");
		return this.http.get(this.conn.ServerUrl + "auth/logout", {
			headers: this.getHeaders()
		}).map(x => x);
	}

	userLoggedinCheck(callback: (loggedin: boolean) => void) {
		if (this.conn && this.conn.ServerUrl) {
			let userJSON = localStorage.getItem("user");
			if (userJSON) {
				let user = JSON.parse(userJSON) as LoginData;
				this.checkLogin(user).subscribe(x => {
					if (x.success) {
						this.loggedInUser = user;
						callback(true);
					} else {
						callback(false);
					}
				}, err => {
					callback(false);
				});
				return;
			}
		}
		callback(false);
	}

	handleError(err: any, title: string, message: string) {
		let body = err.error;
		if (body && body.error) {
			this.handleTypedError(body.error, title, message);
		} else
			this.infoDialog.alert(title, message);
	}

	handleTypedError(err: ErrorItem, title: string, message: string) {
		let msg = err.message || message;
		if (err.errors) {
			err.errors.forEach(x => {
				msg += ` ${x.message}`
			});
		}
		this.infoDialog.alert(title, msg);
	}
}