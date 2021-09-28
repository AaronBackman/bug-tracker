import axios from "axios";
import React from "react";
import Cookies from "universal-cookie";
import { Bar, Chart } from "react-chartjs-2";

import "./DashBoard.css";

class DashBoard extends React.Component {
  state = {
    tickets: [],
  };

  _isMounted = false;

  async componentDidMount() {
    this._isMounted = true;
    const cookies = new Cookies();

    const projectsPromise = await axios.get(
      "https://localhost:5000/api/projects?email=" + cookies.get("email")
    );
    const projects = await projectsPromise.data;
    const promises = [];

    for (const project of projects) {
      promises.push(
        axios.get(
          `https://localhost:5000/api/projects/${
            project.projectGUID
          }/tickets?email=${cookies.get("email")}`
        )
      );
    }

    if (this._isMounted) {
      Promise.all(promises).then((values) => {
        let mergedValues = [];
        for (const value of values) {
          mergedValues = mergedValues.concat(value.data);
        }

        if (this._isMounted) {
          console.log("set state");
          console.log(mergedValues);
          this.setState({ tickets: mergedValues });
        } else {
          console.log("unmounted already");
        }
      });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  render() {
    const cookies = new Cookies();

    console.log(
      this.state.tickets.filter(
        (ticket) =>
          ticket.ticketPriority === 2 &&
          ticket.assignedToEmail === cookies.get("email")
      ).length
    );

    Chart.defaults.font.size = 16;
    Chart.defaults.color = "333";
    Chart.defaults.borderColor = "333";

    const data = {
      labels: ["Low Priority", "Medium Priority", "High Priority"],
      datasets: [
        {
          label: "Tickets",
          data: [
            this.state.tickets.filter(
              (ticket) =>
                ticket.ticketPriority === 0 &&
                ticket.assignedToEmail === cookies.get("email")
            ).length,
            this.state.tickets.filter(
              (ticket) =>
                ticket.ticketPriority === 1 &&
                ticket.assignedToEmail === cookies.get("email")
            ).length,
            this.state.tickets.filter(
              (ticket) =>
                ticket.ticketPriority === 2 &&
                ticket.assignedToEmail === cookies.get("email")
            ).length,
          ],
          backgroundColor: [
            "rgba(75, 192, 192, 0.7)",
            "rgba(255, 206, 86, 0.7)",
            "rgba(255, 99, 132, 0.7)",
          ],
          borderColor: [
            "rgba(75, 192, 192, 1)",
            "rgba(255, 206, 86, 1)",
            "rgba(255, 99, 132, 1)",
          ],
          borderWidth: 1,
        },
      ],
    };

    const options = {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        yAxes: [
          {
            ticks: {
              min: 0,
              stepSize: 1,
            },
          },
        ],
      },
    };

    return (
      <div className="dashboard">
        <div className="ticket-priority-chart-container">
          <Bar data={data} options={options} />
          <div className="chart-text-container">
            <div className="chart-text">Your tickets</div>
          </div>
        </div>
      </div>
    );
  }
}

export default DashBoard;
