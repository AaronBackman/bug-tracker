import React from "react";
import Cookies from "universal-cookie";
import axios from "axios";

import "./ProjectPage.css";

class ProjectPage extends React.Component {
  state = {
    tickets: [],
    projectMembers: [],
    project: {},
  };

  componentDidMount() {
    console.log("mounted project");
    console.log(this.props);
    const cookies = new Cookies();

    axios
      .get(
        `https://localhost:5000/api/projects/${
          this.props.match.params.guid
        }/tickets?email=${cookies.get("email")}`
      )
      .then((response) => {
        console.log(response.data);

        if (response.data) {
          this.setState({ tickets: response.data });
        } else {
          console.log("tickets not found");
        }
      })
      .catch((error) => {
        console.log(error);
      });

    axios
      .get(
        `https://localhost:5000/api/projects/${
          this.props.match.params.guid
        }/members?email=${cookies.get("email")}`
      )
      .then((response) => {
        console.log(response.data);

        if (response.data) {
          this.setState({ projectMembers: response.data });
        } else {
          console.log("members not found");
        }
      })
      .catch((error) => {
        console.log(error);
      });

    axios
      .get(
        `https://localhost:5000/api/projects/${
          this.props.match.params.guid
        }?email=${cookies.get("email")}`
      )
      .then((response) => {
        console.log(response.data);

        if (response.data) {
          this.setState({ project: response.data });
        } else {
          console.log("project not found");
        }
      })
      .catch((error) => {
        console.log(error);
      });
  }

  render() {
    console.log(this.state.tickets);
    console.log(this.state.projectMembers);
    console.log(this.state.project);

    return (
      <div>
        <div>{this.props.match.params.guid}</div>
        <div>{this.state.tickets.length}</div>
        <div>{this.state.projectMembers.length}</div>
        <div>
          <div className="ticket-overview-container">
            <div>Tickets</div>
            <div className="ticket-overview-info">
              Total active tickets:{" "}
              {
                this.state.tickets.filter((ticket) => ticket.ticketCompleted)
                  .length
              }
            </div>
            <div className="ticket-overview-statistics">
              Add charts here using external library
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default ProjectPage;
