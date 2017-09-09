import { Component, OnInit } from '@angular/core';
import { ANAProject, ChatServerConnection } from '../../models/app.models'
import { Router } from '@angular/router';
import { ObjectID } from 'bson';

import { SettingsService } from '../../services/settings.service';
import { ChatFlowService } from '../../services/chatflow.service';
import { GlobalsService } from '../../services/globals.service';
import { MdSnackBar } from '@angular/material';

@Component({
    selector: 'app-projects-manager',
    templateUrl: './projects-manager.component.html',
    styleUrls: ['./projects-manager.component.css']
})
export class ProjectsManagerComponent implements OnInit {
    constructor(
        public settings: SettingsService,
        public chatFlowService: ChatFlowService,
        public global: GlobalsService,
        public snakbar: MdSnackBar,
        public router: Router) { }

    projects: ANAProject[] = [];

    ngOnInit() {
        this.chatFlowService.loadProjectsList().subscribe(resp => {
            if (resp.Data)
                this.projects = resp.Data;
            else
                this.snakbar.open('Unable to load projects! ' + (resp.Message || ''), 'Dismiss');
        }, err => this.handleConnectionFailed(err));

        this.global.currentPageName = 'Projects Manager';
    }

    private handleConnectionFailed(err) {
        if (err.status == 404)
            this.snakbar.open('Connection Failed. Unable to connect to remove server! 404 Not Found.', 'Dismiss');
        else if (err.status == 401)
            this.snakbar.open('Connection Failed. Invalid APIKey and APISecret.', 'Dismiss');
        else
            this.snakbar.open('Connection Failed. ' + (err.data.Message || ''), 'Dismiss');
    }

    load(proj: ANAProject) {
        this.router.navigateByUrl(`/designer/${proj._id}`);
    }

    addProject() {
        this.projects.push({
            _id: new ObjectID().toHexString(),
            CreatedOn: new Date(),
            UpdatedOn: new Date(),
            Name: ''
        });
    }

    saveProjects() {
        this.chatFlowService.saveProjects(this.projects).subscribe(resp => {
            if (resp.Data) {
                this.projects = resp.Data;
                this.snakbar.open('Projects saved.', 'Dismiss');
            }
            else
                this.snakbar.open('Unable to save projects.' + (resp.Message || ''), 'Dismiss');

        }, err => this.handleConnectionFailed(err));
    }

    isExpanded(proj: ANAProject) {
        return this.projects.indexOf(proj) == this.projects.length - 1;
    }
}
