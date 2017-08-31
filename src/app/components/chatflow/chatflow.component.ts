import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Http } from '@angular/http';
import { MdDialog, MdDialogRef, MD_DIALOG_DATA } from '@angular/material';
import { ChatFlowService } from '../../services/chatflow.service'
import * as ChatFlowModels from '../../models/chatflow.models';
import { NodeEditorComponent } from '../nodeeditor/nodeeditor.component';

@Component({
    selector: 'app-chatflow',
    templateUrl: './chatflow.component.html',
    styleUrls: ['./chatflow.component.css'],
})
export class ChatFlowComponent implements AfterViewInit {

    constructor(private chatFlowService: ChatFlowService, public dialog: MdDialog) {
        this.chatFlowNetwork = new ChatFlowNetwork(this);
        this.chatFlowNetwork.newChatNodeConnection.isHidden = true;
        this._viewBoxX = 0;
        this._viewBoxY = 0;
        this._viewBoxWidth = this.designerWidth();
        this._viewBoxHeight = this.designerHeight();

        this.loadChatFlow();
    }
    chatFlowNetwork: ChatFlowNetwork;

    @ViewChild('chatflowRoot')
    chatflowRoot: ElementRef;

    ngAfterViewInit() {
        //async updateLayout to avoid 'ExpressionChangedAfterItHasBeenCheckedError'!
        //Promise.resolve(null).then(() => this.updateLayout());
    }

    chatFlowRootSVG() {
        return this.chatflowRoot.nativeElement as SVGSVGElement;
    }

    _isLayoutUpdated = false;
    updateLayout() {
        if (this.chatFlowNetwork &&
            this.chatFlowNetwork.chatNodeVMs &&
            this.chatFlowNetwork.chatNodeVMs.length > 0 &&
            this.chatflowRoot) {
            let ele = this.chatFlowRootSVG();
            if (ele.querySelector) { //Initialization issues, proceed only if querySelector is available.

                if (window && window.screen) {
                    this._designerHeight = window.screen.height - Config.designerMargin;
                    this._designerWidth = window.screen.width - Config.designerMargin;
                }

                for (let i = 0; i < this.chatFlowNetwork.chatNodeVMs.length; i++) {
                    let x = this.chatFlowNetwork.chatNodeVMs[i];

                    let btnsTable = this.chatFlowRootSVG().querySelector(`table[node-id='${x.chatNode.Id}']`) as HTMLTableElement;
                    if (btnsTable) {
                        x._btnTableWidth = btnsTable.getBoundingClientRect().width;//btnsTable.clientWidth;//
                        x._width = ((x._width > x._btnTableWidth) ? x._width : x._btnTableWidth);

                        setTimeout(() => {
                            let nodeRoot = this.chatFlowRootSVG().querySelector(`div[node-id='${x.chatNode.Id}']`) as HTMLDivElement;
                            x._height = nodeRoot.clientHeight;
                            this._isLayoutUpdated = true;
                        }, 500);
                    }
                    else {
                        setTimeout(() => this.updateLayout(), 500); //Document not ready yet! Wait 500ms
                        break;
                    }
                }

            }
        }
    }

    ngTr(x: number, y: number) {
        return `translate(${x},${y})`;
    }

    _designerHeight: number = Config.defaultDesignerHeight;
    designerHeight() {
        return this._designerHeight;
    }

    _designerWidth: number = Config.defaultDesignerWidth;
    designerWidth() {
        return this._designerWidth;
    }

    mouseMove(event: MouseEvent) {
        if (!this.chatFlowNetwork.newChatNodeConnection.isHidden) {
            let targetXY = this.transformCoordinates(event.pageX, event.pageY, event);
            this.chatFlowNetwork.newChatNodeConnection.destX = targetXY.x - 30;
            this.chatFlowNetwork.newChatNodeConnection.destY = targetXY.y - 30;
        }

        if (this.chatFlowNetwork.draggingChatNode) {
            try {
                let targetXY = this.transformCoordinates(event.pageX, event.pageY, event);
                let offset = this.chatFlowNetwork.draggingChatNodeOffset;
                this.chatFlowNetwork.draggingChatNode._x = targetXY.x - offset.x;
                this.chatFlowNetwork.draggingChatNode._y = targetXY.y - offset.y;
            } catch (e) {
                this.chatFlowNetwork.draggingChatNode._x += event.movementX;
                this.chatFlowNetwork.draggingChatNode._y += event.movementY;
            }
        }

        if (this._isMouseDown) {
            this._viewBoxX -= event.movementX;
            this._viewBoxY -= event.movementY;
        }
    }

    transformCoordinates(x: number, y: number, event: MouseEvent) {
        let svg_elem = this.chatFlowRootSVG();
        let matrix = svg_elem.getScreenCTM();
        let point = svg_elem.createSVGPoint();
        point.x = x - event.view.pageXOffset;
        point.y = y - event.view.pageYOffset;
        return point.matrixTransform(matrix.inverse());
    }

    _isMouseDown = false;
    mouseDown(event: MouseEvent) {
        //Check if mouse is captured by others
        if (this.chatFlowNetwork.newChatNodeConnection.isHidden && !this.chatFlowNetwork.draggingChatNode)
            this._isMouseDown = true;
        else
            this._isMouseDown = false;
    }

    mouseUp(event: MouseEvent) {
        this.resetDraggingState();
    }

    mouseLeave(event: MouseEvent) {
        this.resetDraggingState();
    }

    _viewBoxWidth: number;
    _viewBoxHeight: number;

    _viewBoxX: number;
    _viewBoxY: number;
    viewBox() {
        //0 0 1000 500
        return `${this._viewBoxX} ${this._viewBoxY} ${this._viewBoxWidth} ${this._viewBoxHeight}`;
    }

    designerWheel(event: WheelEvent) {
        event.preventDefault();
        console.log(event.wheelDelta);

        let change = Config.zoomCoefficient * event.wheelDelta;
        if (this._viewBoxWidth - change < 0 || this._viewBoxHeight - change < 0) {
            return;
        }

        this._viewBoxWidth -= change;
        this._viewBoxHeight -= change;

        console.log(this.viewBox());
    }

    openEditor(chatNodeVM: ChatNodeVM) {
        let dialogRef = this.dialog.open(NodeEditorComponent, {
            width: '80%',
            data: chatNodeVM
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }

    private resetDraggingState() {
        if (!this.chatFlowNetwork.newChatNodeConnection.isHidden)
            this.chatFlowNetwork.newChatNodeConnection.isHidden = true;
        if (this.chatFlowNetwork.draggingChatNode)
            delete this.chatFlowNetwork.draggingChatNode;
        this._isMouseDown = false;
    }
    //599647fa460b500d9c4cb11c
    private loadChatFlow(projectId: string = '599c3caa460b5053e4b09869') { //5996231a460b50abbc083b71
        //this.chatFlowService.loadProjectsList().subscribe(projects => {
        //    console.log(projects.Data.map(x => x.Name).join(", "));
        //}, err => console.error(err));
        this.chatFlowService.fetchChatFlowPack(projectId).subscribe(x => {
            x.ChatNodes.forEach(cn => {
                new ChatNodeVM(this.chatFlowNetwork, cn);
            });
            this.chatFlowNetwork.updateChatNodeConnections();
            this.updateLayout();

        }, err => console.error(err));
    }
}

class ChatFlowNetwork {
    constructor(public parent: ChatFlowComponent) {
    }

    updateChatNodeConnections(): void {
        this.chatNodeConnections = [];

        this.chatNodeVMs.forEach(chatNodeVM => {
            chatNodeVM.chatNode.Buttons.forEach(srcBtn => {
                if (srcBtn.NextNodeId != null || srcBtn.NextNodeId != "") {
                    let destNode = this.chatNodeVMs.filter(x => x.chatNode.Id == srcBtn.NextNodeId);
                    if (destNode && destNode.length > 0)
                        this.chatNodeConnections.push(new ChatNodeConnection(new ChatButtonConnector(chatNodeVM, srcBtn), destNode[0]));
                }
            });
        });
    }

    chatNodeConnections: ChatNodeConnection[] = [];
    chatNodeVMs: ChatNodeVM[] = [];

    newChatNodeConnection: ChatNodeNewConnection = new ChatNodeNewConnection();
    draggingChatNode: ChatNodeVM;
    draggingChatNodeOffset: Point;
}

class ChatNodeConnection {
    constructor(
        public srcButtonConnector: ChatButtonConnector,
        public destChatNodeVM: ChatNodeVM) {
    }

    srcConnectorX() {
        return this.srcButtonConnector.x() - (this.pathWidth / 2);
    }
    srcConnectorY() {
        return this.srcButtonConnector.y();
    }

    destConnectorX() {
        return this.destChatNodeVM.nodeConnectorX() - (this.pathWidth / 2)
    }
    destConnectorY() {
        return this.destChatNodeVM.nodeConnectorY();
    }

    calcTangentOffset(pt1X: number, pt2X: number) {
        return ((pt2X - pt1X) / 2);
    }

    calcSrcTangentX() {
        let pt1X = this.srcConnectorX();
        let pt2X = this.destConnectorX();
        return pt1X + this.calcTangentOffset(pt1X, pt2X);
    }
    calcSrcTangentY() {
        return this.srcConnectorY();
    }

    calcDestTangentX() {
        let pt1X = this.srcConnectorX();
        let pt2X = this.destConnectorX();
        return pt2X - this.calcTangentOffset(pt1X, pt2X);
    }
    calcDestTangentY() {
        return this.destConnectorY();
    }

    path() {
        return `M${this.srcConnectorX()},${this.srcConnectorY()} 
                C${this.calcSrcTangentX()},${this.calcSrcTangentY()} 
                  ${this.calcDestTangentX()},${this.calcDestTangentY()} 
                ${this.destConnectorX()},${this.destConnectorY()}`;
    }

    closeButtonVisible = false;
    closeButtonPointX: number = 0;
    closeButtonPointY: number = 0;
    mouseEnter(event: MouseEvent) {
        let xy = this.destChatNodeVM.network.parent.transformCoordinates(event.pageX, event.pageY, event);
        this.closeButtonPointX = xy.x; //some offset from the cursor
        this.closeButtonPointY = xy.y; //some offset from the cursor
        this.closeButtonVisible = true;
        console.log(`${this.closeButtonPointX},${this.closeButtonPointY}`);
        //alert(`${this.closeButtonPoint.x},${this.closeButtonPoint.y}`);
        setTimeout(() => {
            this.closeButtonVisible = false;
        }, 50000); //Hide the close button after 5secs
    }

    circleRadius = Config.buttonCircleRadius;
    pathWidth = Config.connectionPathWidth;
}

class ChatNodeNewConnection {
    srcButtonConnector: ChatButtonConnector;
    destX: number;
    destY: number;

    isHidden = false;
    canConnect = false;

    srcConnectorX() {
        if (this.srcButtonConnector)
            return this.srcButtonConnector.x();
        return 0;
    }
    srcConnectorY() {
        if (this.srcButtonConnector)
            return this.srcButtonConnector.y();
        return 0;
    }

    calcTangentOffset(pt1X: number, pt2X: number) {
        return ((pt2X - pt1X) / 2);
    }

    calcSrcTangentX() {
        let pt1X = this.srcConnectorX();
        let pt2X = this.destX;
        return pt1X + this.calcTangentOffset(pt1X, pt2X);
    }
    calcSrcTangentY() {
        return this.srcConnectorY();
    }

    calcDestTangentX() {
        let pt1X = this.srcConnectorX();
        let pt2X = this.destX;
        return pt2X - this.calcTangentOffset(pt1X, pt2X);
    }
    calcDestTangentY() {
        return this.destY;
    }

    path() {
        if (this.isHidden)
            return "M 0,0";

        return `M${this.srcConnectorX()},${this.srcConnectorY()} 
                C${this.calcSrcTangentX()},${this.calcSrcTangentY()} 
                  ${this.calcDestTangentX()},${this.calcDestTangentY()} 
                ${this.destX},${this.destY}`;
    }

    circleRadius = Config.buttonCircleRadius;
}

class ChatButtonConnector {
    constructor(
        public chatNodeVM: ChatNodeVM,
        public button: ChatFlowModels.Button) {
    }

    x() {
        let btns = this.chatNodeVM.chatNode.Buttons;
        let btnsCount = btns.length;
        let eachPart = (this.chatNodeVM.width() / btnsCount);
        let _x = (
            this.chatNodeVM.x()
            + ((eachPart) * (this.btnIndex() + 1))
            - eachPart / 2
            - this.chatNodeVM.circleRadius
        );
        return _x;
    }

    y() {
        return this.chatNodeVM.y() + this.chatNodeVM.height();
    }

    circleRadius = Config.buttonCircleRadius;

    mouseDown(event: MouseEvent) {
        let nw = this.chatNodeVM.network;
        if (nw.newChatNodeConnection.isHidden)
            nw.newChatNodeConnection.isHidden = false;
        nw.newChatNodeConnection.srcButtonConnector = this;
        nw.newChatNodeConnection.destX = this.x();
        nw.newChatNodeConnection.destY = this.y();
    }

    btnIndex() {
        let btns = this.chatNodeVM.chatNode.Buttons;
        return btns.indexOf(this.button);
    }

    setButtonNextNodeId(nextNodeId: string) {
        this.button.NextNodeId = nextNodeId;
        this.chatNodeVM.network.updateChatNodeConnections();
    }
}

export class ChatNodeVM {
    constructor(
        public network: ChatFlowNetwork,
        public chatNode: ChatFlowModels.ChatNode) {
        this.network.chatNodeVMs.push(this);

        this._x = (this.network.chatNodeVMs.indexOf(this)) * Config.defaultNodeWidth;
    }

    _x: number = 0;
    _y: number = 0;

    _btnTableWidth: number = Config.defaultNodeWidth;
    _width: number = Config.defaultNodeWidth;
    width() {
        return this._width;
    }

    _height: number = Config.defaultNodeHeight;
    height() {
        return this._height;
    }

    x() {
        return this._x;
    }

    y() {
        return this._y;
    }

    cornerRadius: number = Config.defaultNodeCornerRadius;
    headerHeight: number = Config.defaultNodeHeaderHeight;

    mouseDown(event: MouseEvent) {
        this.network.draggingChatNode = this;

        let targetXY = this.network.parent.transformCoordinates(event.pageX, event.pageY, event);
        let mouseOffsetX = targetXY.x - this._x;
        let mouseOffsetY = targetXY.y - this._y;
        this.network.draggingChatNodeOffset = new Point(mouseOffsetX, mouseOffsetY);
    }

    mouseUp(event: MouseEvent) {
        let nw = this.network;
        if (!nw.newChatNodeConnection.isHidden) {
            if (nw.newChatNodeConnection.srcButtonConnector.chatNodeVM != this)
                nw.newChatNodeConnection.srcButtonConnector.setButtonNextNodeId(this.chatNode.Id);
        }
    }

    mouseEnter(event: MouseEvent) {
        let nw = this.network;
        if (!nw.newChatNodeConnection.isHidden) {
            if (nw.newChatNodeConnection.srcButtonConnector.chatNodeVM != this)
                nw.newChatNodeConnection.canConnect = true;
        }
    }

    mouseLeave(event: MouseEvent) {
        let nw = this.network;
        if (!nw.newChatNodeConnection.isHidden) {
            nw.newChatNodeConnection.canConnect = false;
        }
    }

    chatButtonConnectors() {
        return this.chatNode.Buttons.map(btn => new ChatButtonConnector(this, btn));
    }

    nodeConnectorY() {
        return this.y();
    }

    nodeConnectorX() {
        return (this.x()) + (this.width() / 2) - this.circleRadius;
    }

    circleRadius = Config.buttonCircleRadius;
}

class Point {
    constructor(public x: number, public y: number) { }
}

class Config {
    static defaultNodeWidth = 300;
    static defaultNodeHeight = 200;
    static defaultNodeHeaderHeight = 30;

    static defaultNodeCornerRadius = 20;

    static defaultSectionWidth = 30;
    static defaultSectionHeight = 30;

    static defaultDesignerWidth = 1366;
    static defaultDesignerHeight = 700;

    static buttonCircleRadius = 8;
    static designerMargin = 40;
    static connectionPathWidth = 3;

    static zoomCoefficient = 0.3;
}
