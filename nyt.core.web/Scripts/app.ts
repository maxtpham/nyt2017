import * as $ from "jquery"
import * as moment from "moment"
import { UserServiceAPI } from "./UserServiceAPI"
import { TaskServiceAPI } from "./TaskServiceAPI"

class App {
    private container: HTMLDivElement;
    private loading: HTMLElement;
    private taskTable: HTMLTableElement;

    constructor() {
        this.container = <HTMLDivElement>$("#app").get(0);
        if (this.container == null) {
            this.main = () => { };
        } else {
            this.loading = <HTMLElement>this.container.children[0];
        }
    }

    main() {
        this.createTaskForm();
        this.taskTable = this.createTasksPanel();
    }

    private onLoaded() {
        if (this.loading != null) {
            this.loading.remove();
            this.loading = undefined;
        }
    }

    private createTaskForm() {
        let form = this.container.appendChild(document.createElement("form"));
        form.innerHTML = `
  <div class="form-group">
    <label for="taskCode">Task Code</label>
    <input type="text" class="form-control" id="taskCode" placeholder="Code">
  </div>
  <div class="form-group">
    <label for="taskTitle">Task Title</label>
    <input type="text" class="form-control" id="taskTitle" placeholder="Title">
  </div>
  <div class="form-group">
    <label for="taskContent">Task Content</label>
    <input type="textarea" class="form-control" id="taskContent" placeholder="Details..">
  </div>
  <button type="button" class="btn btn-default" id="taskButton">Create Task</button>
  <hr/>
        `;
        $("#taskButton").click(this.onCreateTask.bind(this));
    }

    private onCreateTask() {
        let taskClient = new TaskServiceAPI.Client("http://taskservice.dev");
        let task = <TaskServiceAPI.ITask> {
            code: $("#taskCode").val(),
            title: $("#taskTitle").val(),
            content: $("#taskContent").val(),
        };
        if (task.code == null || task.code.trim().length <= 0) {
            alert("Code is required!");
            $("#taskCode").focus();
        } else if (task.title == null || task.title.trim().length <= 0) {
            alert("Title is required!");
            $("#taskTitle").focus();
        } else {
            taskClient.apiTasksPost(new TaskServiceAPI.Task(task)).then(() => {
                alert("Task is created successfully!");
                $("#taskCode").val("");
                $("#taskTitle").val("");
                $("#taskContent").val("");
                $("#taskCode").focus();
                this.taskTable.parentElement.remove();
                this.taskTable = this.createTasksPanel();
            });
        }
    }

    private createTasksPanel(): HTMLTableElement {
        let table = this.createTable("Tasks");
        this.createTableRow(table.tHead, ["ID", "Code", "Title", "Content", "Created", "Updated"], true);

        let taskClient = new TaskServiceAPI.Client("http://taskservice.dev");
        taskClient.apiTasksGet().then(tasks => {
            tasks.forEach(task => {
                this.createTableRow(
                    <HTMLTableSectionElement>table.tBodies[0],
                    [this.toStringElipsis(task.id, 7), this.toStringElipsis(task.code, 10), this.toStringElipsis(task.title, 40), this.toStringElipsis(task.content, 20), moment(task.created).fromNow(true), moment(task.updated).fromNow(true)]
                );
            });
            this.onLoaded();
        });

        return table;
    }

    private toStringElipsis(value: any, length: number = 10): string {
        if (!value)
            return "";
        let str = String(value);
        return (length <= 2 || str.length <= length) ? str : (str.substr(0, length - 2) + "..");
    }

    private createTable(header: string): HTMLTableElement {
        let panel = this.container.appendChild(document.createElement("div"));
        panel.className = "panel panel-primary table-responsive";

        let heading = panel.appendChild(document.createElement("div"));
        heading.className = "panel-heading";
        heading.innerText = header;

        let table = panel.appendChild(<HTMLTableElement>document.createElement("table"));
        table.className = "table";
        table.createTHead();
        table.createTBody();
        return table;
    }

    private createTableRow(section: HTMLTableSectionElement, values: any[], heading: boolean = false): HTMLTableRowElement {
        let row = section.insertRow();
        values.forEach((v, i, arr) => {
            let cell: HTMLTableCellElement;
            if (heading) {
                cell = <HTMLTableHeaderCellElement>document.createElement("th");
                row.insertBefore(cell, undefined);
            } else {
                cell = row.insertCell();
            }
            cell.innerHTML = v;
        });
        return row;
    }
}

$(document).ready(() => new App().main());