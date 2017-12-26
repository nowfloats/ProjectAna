import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { InfoDialogService } from './info-dialog.service';
import { ChatServerConnection, ChatBotProject } from '../models/app.models';

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

	getBusinessAccounts() {
		let h = this.getHeaders();
		return this.http.get(this.conn.ServerUrl + "business/accounts", { headers: h })
			.map(x => x as APIResponse<ListData<BusinessAccount>>);
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

	handleTypedError(err: Error, title: string, message: string) {
		let msg = err.message || message;
		if (err.errors) {
			err.errors.forEach(x => {
				msg += ` ${x.message}` 
			});
		}
		this.infoDialog.alert(title, msg);
	}
}

export interface ErrorDetail {
	field: string;
	message: string;
}

export interface Error {
	code: string;
	status: number;
	message: string;
	timestamp: number;
	errors: ErrorDetail[];
}

export interface APIResponse<TData> {
	data?: TData;
	error?: Error;
	success: boolean;
	links: Link[];
}

export interface Link {
	href: string;
	rel: string;
	templated: boolean;
}
export interface Role {
	id: number;
	role: string;
	description: string;
	label: string;
	enabled: boolean;
}

export interface LoginData {
	userId: string;
	username: string;
	accessToken: string;
	name: string;
	businessId: string;
	roles: Role[];
}

export interface Color {
	id?: string;
	name: string;
	value: string;
}

export interface BusinessAccount {
	colors: Color[];
	createdAt?: number;
	email: string;
	id?: string;
	logoUrl: string;
	modifiedAt?: number;
	name: string;
	phone: string;
	registerByUserId?: string;
	status?: string;
	password?: string;
	fullName?: string;
	userEmail?: string;
	userPhone?: string;
}

export interface Sort {
}

export interface ListData<TItem> {
	content: TItem[];
	first: boolean;
	last: boolean;
	number: number;
	numberOfElements: number;
	size: number;
	sort: Sort;
	totalElements: number;
	totalPages: number;
}

export enum BusinessAccountStatus {
	INACTIVE = 0,
	ACTIVE = 1,
	EXPIRED = 'EXPIRED',
	BLOCKED = 'BLOCKED',
	DELETED = 'DELETED'
}