import axios from 'axios';
import React from 'react';
import Cookies from 'universal-cookie';
import {Bar} from 'react-chartjs-2';

import './DashBoard.css';

class DashBoard extends React.Component {
    state = {
        tickets: []
    }

    async componentDidMount() {
        const cookies = new Cookies();

        const projectsPromise = await axios.get('https://localhost:5000/api/projects?email=' + cookies.get('email'));
        const projects = await projectsPromise.data;
        console.log(projects);
        const promises = [];

        for (const project of projects) {
            promises.push(axios.get(`https://localhost:5000/api/projects/${project.projectGUID}/tickets?email=${cookies.get('email')}`));
        }

        const ticketPromises = await Promise.all(promises);
        const tickets = [];

        for (const ticketPromise of ticketPromises) {
            tickets.concat(ticketPromise.data);
        }

        console.log(tickets);
    }

    render() {
        const data = {
            labels: ['Low', 'Medium', 'High'],
            datasets: [
                {
                    label: 'Tickets by Priority',
                    data: [
                        this.state.tickets.filter(ticket => ticket.priority === 0),
                        this.state.tickets.filter(ticket => ticket.priority === 1),
                        this.state.tickets.filter(ticket => ticket.priority === 2)
                    ],
                    backgroundColor: [
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(255, 99, 132, 0.2)'
                    ],
                    borderColor: [
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }
            ]
        }

        const options = {
            scales: {
              yAxes: [
                {
                  ticks: {
                    beginAtZero: true,
                  },
                },
              ],
            },
          };

        return (
            <div>
                <div className="ticket-priority-chart-container">
                    <Bar data={data} options={options} />
                </div>
            </div>
        );
    }
}

export default DashBoard;