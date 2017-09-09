import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChatServerConnection } from '../../models/app.models';
import { SettingsService } from '../../services/settings.service';
import { ChatFlowService } from '../../services/chatflow.service';
import { GlobalsService } from '../../services/globals.service';
import { MdSnackBar } from '@angular/material';

@Component({
    selector: 'app-chat-server-manager',
    templateUrl: './chat-server-manager.component.html',
    styleUrls: ['./chat-server-manager.component.css']
})
export class ChatServerManagerComponent implements OnInit {
    constructor(
        public settings: SettingsService,
        public chatFlowService: ChatFlowService,
        public global: GlobalsService,
        public snakbar: MdSnackBar,
        public router: Router) { }

    savedConnections: ChatServerConnection[] = [];

    ngOnInit(): void {
        this.savedConnections = this.settings.loadSavedConnections();
        this.global.currentPageName = 'Chat Server Manager';
    }

    testConnection(conn: ChatServerConnection) {
        this.global.loading = true;
        this.chatFlowService.testChatServerConnection(conn).subscribe(resp => {
            this.global.loading = false;

            this.snakbar.open('Connection Successful', 'Dismiss');
        }, err => {
            this.global.loading = false;

            this.handleConnectionFailed(err);
        }, () => {
            this.global.loading = false;
        });
    }

    connectToConnection(conn: ChatServerConnection) {
        this.chatFlowService.testChatServerConnection(conn).subscribe(resp => {
            this.chatFlowService.setChatServer(conn);

            this.router.navigateByUrl('/projects');
        }, err => this.handleConnectionFailed(err));
    }

    private handleConnectionFailed(err) {
        console.log(err);
        if (err.status == 404)
            this.snakbar.open('Connection Failed. Unable to connect to remove server! 404 Not Found.', 'Dismiss');
        else if (err.status == 401)
            this.snakbar.open('Connection Failed. Invalid APIKey and APISecret.', 'Dismiss');
        else
            this.snakbar.open('Connection Failed.' + (err.data.Message || ''), 'Dismiss');
    }

    connectionAlias(conn: ChatServerConnection) {
        return conn.Name || conn.ServerUrl || 'Unnamed Connection';
    }

    deleteConnection(conn: ChatServerConnection) {
        var current = this.savedConnections.indexOf(conn);
        if (current != -1) {
            if (confirm(`Delete chat server connection '${this.connectionAlias(conn)}'`))
                this.savedConnections.splice(current, 1);
        }
    }

    saveConnections() {
        this.settings.saveSavedConnections(this.savedConnections);

        this.snakbar.open('Connections saved!', 'Dismiss', {
            duration: 3000
        });
    }

    addConnection() {
        let newConn = {
            APIKey: '',
            APISecret: '',
            IsDefault: false,
            Name: '',
            ServerUrl: '',
        };
        this.savedConnections.push(newConn);
    }

    isExpanded(conn: ChatServerConnection) {
        return this.savedConnections.indexOf(conn) == this.savedConnections.length - 1;
    }
}
