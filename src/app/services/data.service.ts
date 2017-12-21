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
		let h = new HttpHeaders();
		if (this.loggedInUser && this.loggedInUser.accessToken)
			h.set("access-token", this.loggedInUser.accessToken)
		return h;
	}

	setConnection(conn: ChatServerConnection) {
		if (conn && conn.ServerUrl)
			conn.ServerUrl = this.normalizeBaseUrl(conn.ServerUrl);
		localStorage.setItem("conn", JSON.stringify(conn));
		this.conn = conn;
	}

	getUserAccounts() {

	}

	createUserAccount() {

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
		if (err._body) {
			try {
				let respMsg = JSON.parse(err._body) as APIResponse<any>;
				this.handleTypedError(respMsg.error, title, message);
			} catch (e) {
				this.infoDialog.alert(title, err._body || message);
			}
		} else
			this.infoDialog.alert(title, message);
	}

	handleTypedError(err: Error, title: string, message: string) {
		this.infoDialog.alert(title, err.message || message);
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
	links: any[];
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