import { Injectable } from '@angular/core';
import { Http, RequestOptionsArgs, Headers } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import * as models from '../models/chatflow.models'
import * as ServiceResponseModels from '../models/serviceResponse.models'
import { ChatServerConnection, ANAProject } from '../models/app.models';

@Injectable()
export class ChatFlowService {
    constructor(private http: Http) { }

    //Chat server base URL
    private baseUrl: string; //= "http://ria.nowfloatsdev.com/chat4/"; //"http://localhost:2646/";
    private auth: string;// = 'Basic bm93ZmxvYXRzOm5vd2Zsb2F0czEyMw==';

    private loadProjectsAPI: string = "api/Project/List";
    private saveProjectsAPI: string = "api/Project/Save";
    private saveChatFlowPackAPI: string = "api/Conversation/SaveChatFlow";
    private fetchChatFlowPackAPI: string = "api/Conversation/FetchChatFlowPack?projectId={projectId}";

    private getHttpOptions() {
        var headers = new Headers();
        headers.set('Authorization', this.auth)
        return {
            headers: headers,
        };
    }

    loadProjectsList() {
        return this.http.get(this.baseUrl + this.loadProjectsAPI, this.getHttpOptions()).map(res =>
            (res.json() as ServiceResponseModels.ProjectListResponse));
    }

    fetchChatFlowPack(projectId: string) {
        return this.http.get(this.baseUrl + this.fetchChatFlowPackAPI.replace("{projectId}", projectId), this.getHttpOptions()).map(res =>
            res.json() as models.ChatFlowPack);
    }

    saveProjects(projects: ANAProject[]) {
        return this.http.post(this.baseUrl + this.saveProjectsAPI, projects, this.getHttpOptions()).map(res =>
            (res.json() as ServiceResponseModels.ProjectListResponse));
    }

    setChatServer(conn: ChatServerConnection) {
        if (!conn.ServerUrl) {
            throw 'Server Url in the given connection is empty!';
        }

        this.baseUrl = conn.ServerUrl;
        this.baseUrl = this.normalizeBaseUrl(this.baseUrl);
        if (conn.APIKey || conn.APISecret)
            this.auth = this.getAuth(conn);
    }

    private normalizeBaseUrl(baseUrl: string) {
        baseUrl = baseUrl.replace(/\\$/, '');//Remove ending \ char if any
        if (!baseUrl.endsWith('/'))
            baseUrl += '/';
        return baseUrl;
    }

    private getAuth(conn: ChatServerConnection) {
        return 'Basic ' + btoa(`${conn.APIKey || ''}:${conn.APISecret || ''}`);
    }

    testChatServerConnection(conn: ChatServerConnection) {
        var headers = new Headers();
        headers.set('Authorization', this.getAuth(conn))

        return this.http.get(this.normalizeBaseUrl(conn.ServerUrl) + this.loadProjectsAPI, { headers: headers }).map(res =>
            res.json() as ServiceResponseModels.ProjectListResponse)
    }
}
