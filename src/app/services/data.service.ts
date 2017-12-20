import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { ChatServerConnection, ChatBotProject } from '../models/app.models';

@Injectable()
export class DataService {

	constructor(private http: Http) { }
	connection: ChatServerConnection;

	getUserAccounts() {

	}

	createUserAccount() {

	}


}