import { useEffect, useState } from 'react';
import 'react-date-range/dist/styles.css'; // main style file
import 'react-date-range/dist/theme/default.css'; // theme css file

import './App.css';
import { DateRangePicker } from 'react-date-range';

const selectionRange = {
    startDate: new Date(),
    endDate: addDays(1),
    key: 'selection',
}

function addDays(days) {
    var date = new Date()
    date.setDate(date.getDate() + days);
    return date;
}

function offsetTimezone(date) {
    let offset = date.getTimezoneOffset();
    const adjustedDate = new Date(date.getTime() - offset * 60000);
    return adjustedDate;
}

function App() {

    const [freeRooms, setFreeRooms] = useState();
    const [bookedRooms, setBookedRooms] = useState();
    const [selectedView, setSelectedView] = useState();
    const [allReservations, setAllReservations] = useState();
    const [inputId, setInputId] = useState();
    const [firstName, setFirstName] = useState();
    const [lastName, setLastName] = useState();
    const [email, setEmail] = useState();

    const serverUrl = "http://localhost:8080/";

    useEffect(() => {
        handleSelect();
        addDays();
        bookRoom();
    }, []);

    const handleIdChange = (event) => {
        setInputId(event.target.value);
    };

    const handleFirstNameChange = (event) => {
        setFirstName(event.target.value);
    };

    const handleLastNameChange = (event) => {
        setLastName(event.target.value);
    };

    const handleEmailChange = (event) => {
        setEmail(event.target.value);
    };

    const currentView = !selectedView ? "Customer" : selectedView;
    const currentContent = currentView === "Customer" ? getCustomerView() : getBackofficeView();

    return <div>
        {getBackofficeSwitchContent(currentView)}
        {currentContent}
    </div>;

    function getBackofficeView() {
        const reservations = allReservations === undefined
            ?
            <div></div>
            :
            <div>
                <table className="table table-striped" aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Reserver Id Code</th>
                            <th>Room number</th>
                            <th>Checkin date</th>
                            <th>Checkout date</th>
                            <th>Sleeping spots</th>
                            <th>Nightly Price</th>
                            <th>Update</th>
                            <th>Cancel</th>
                        </tr>
                    </thead>
                    <tbody>
                        {allReservations.map(reservation =>
                            <tr key={reservation.id}>
                                <td>{reservation.reserverIdCode}</td>
                                <td>{reservation.roomNumber}</td>
                                <td><input id={"start_reservation" + reservation.id} placeholder={reservation.reservationStartDate} /></td>
                                <td><input id={"end_reservation" + reservation.id} placeholder={reservation.reservationEndDate} /></td>
                                <td>{reservation.sleepSpotCount}</td>
                                <td>{reservation.nightlyPrice}</td>
                                <td><button onClick={() => updateReservation(reservation.id)}>Update</button></td>
                                <td><button onClick={() => cancelReservation(reservation.id)}>Cancel</button></td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>;

        return <div>
            <h1>Backoffice</h1>
            <button onClick={() => populateReservations()}>Fetch all reservations</button >
            {reservations}
        </div>
    }

    function getBackofficeSwitchContent(currentView) {
        return <div>
            <p>Change between backoffice and customer view</p>
            <p>CurrentView is {currentView}</p>
            <button onClick={() => setSelectedView("Backoffice")}>Backoffice</button>
            <button onClick={() => setSelectedView("Customer")}>Customer</button>
        </div>
    }

    function getCustomerView() {
        const contents =
            <DateRangePicker
                ranges={[selectionRange]}
                onChange={handleSelect}
            />

        const availableRoomsContent = freeRooms === undefined
            ? <p>Free Rooms not requested!</p>
            :
            <div>
                <p>Choose room for the selected dates</p>
                <table className="table table-striped" aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Room number</th>
                            <th>Nightly Price</th>
                            <th>Sleeping spots</th>
                        </tr>
                    </thead>
                    <tbody>
                        {freeRooms.map(freeRoom =>
                            <tr key={freeRoom.id}>
                                <td>{freeRoom.roomNumber}</td>
                                <td>{freeRoom.nightlyPrice}</td>
                                <td>{freeRoom.sleepSpotCount}</td>
                                <td><button onClick={() => bookRoom(freeRoom.id)}>Select Room</button></td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

        const bookedRoomsContent = bookedRooms === undefined
            ? <div></div>
            :
            <div>
                <table className="table table-striped" aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Room number</th>
                            <th>Checkin date</th>
                            <th>Checkout date</th>
                            <th>Sleeping spots</th>
                            <th>Nightly Price</th>
                            <th>Cancel Reservation?</th>
                        </tr>
                    </thead>
                    <tbody>
                        {bookedRooms.map(reservation =>
                            <tr key={reservation.id}>
                                <td>{reservation.roomNumber}</td>
                                <td>{reservation.reservationStartDate.substring(0, 10)}</td>
                                <td>{reservation.reservationEndDate.substring(0, 10)}</td>
                                <td>{reservation.sleepSpotCount}</td>
                                <td>{reservation.nightlyPrice}</td>
                                <td><button onClick={() => cancelBooking(reservation.id)}>Cancel</button></td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        return (
            <div>
                {getCustomerContent()}

                <div>
                    <h1>Available rooms</h1>
                    <p>Please select booking range to get more details on available rooms</p>
                    {contents}
                    <button onClick={getFreeRooms}>Confirm booking range</button>
                </div>
                <div>
                    {availableRoomsContent}
                </div>
                <div>
                    <h1>Booked rooms</h1>
                    <button onClick={() => getBookedRooms()}>Fetch Rooms for {inputId}</button>
                    {bookedRoomsContent}
                </div>
            </div>
        );
    }

    function getCustomerContent() {
        return <div>
            <h1>Your data</h1>
            <form>
                <label>Id Code
                    <input type="number" min="10001010001" max="99999999999" placeholder="10001010013" onChange={handleIdChange} />
                </label><br/>
                <label>First Name
                    <input type="text" onChange={handleFirstNameChange}></input>
                </label><br/>
                <label>Last Name
                    <input type="text" onChange={handleLastNameChange}></input>
                </label><br/>
                <label>Email
                    <input type="email" onChange={handleEmailChange}></input>
                </label><br/>
            </form>

        </div>
    }

    function handleSelect(ranges) {
        if (ranges == undefined) {
            return;
        }
        
        selectionRange.startDate = ranges.selection.startDate
        selectionRange.endDate = ranges.selection.endDate
        
    }

    async function getFreeRooms() {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", serverUrl + "booking/free");
        setXhrRequestHeaders(xhr);

        const body = JSON.stringify({
            StartDate: offsetTimezone(selectionRange.startDate),
            EndDate: offsetTimezone(selectionRange.endDate)
        });
        xhr.onload = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                console.log("Success, response: " + JSON.stringify(xhr.responseText));
                setFreeRooms(JSON.parse(xhr.responseText));

            } else {
                handleXhrError(xhr);
            }
        };
        xhr.send(body);
    }

    async function bookRoom(roomId) {
        if (roomId === undefined) {
            return;
        }
        const xhr = new XMLHttpRequest();
        xhr.open("POST", serverUrl + "booking/book");
        setXhrRequestHeaders(xhr);
        const body = JSON.stringify({
            StartDate: offsetTimezone(selectionRange.startDate),
            EndDate: offsetTimezone(selectionRange.endDate),
            RoomId: roomId,
            IdNumber: inputId,
            FirstName: firstName,
            LastName: lastName,
            Email: email
        });
        xhr.onload = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                console.log("Success, response: " + JSON.stringify(xhr.responseText));
                getFreeRooms();
            } else {
                handleXhrError(xhr);
            }
        };
        xhr.send(body);
    }

    async function getBookedRooms() {
        const xhr = new XMLHttpRequest();
        xhr.open("GET", serverUrl + "booking/booked/" + inputId);
        setXhrRequestHeaders(xhr);
        xhr.onload = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                console.log("Success, response: " + JSON.stringify(xhr.responseText));
                const response = JSON.parse(xhr.responseText);
                setBookedRooms(response.bookedRooms);
            } else {
                handleXhrError(xhr);
            }
        };
        xhr.send();
    }

    async function cancelBooking(reservationId) {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", serverUrl + "booking/cancel");
        setXhrRequestHeaders(xhr);
        const body = JSON.stringify({
            ReservationId: reservationId,
        });
        xhr.onload = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                console.log("Success, response: " + JSON.stringify(xhr.responseText));
                const response = JSON.parse(xhr.responseText);
                setBookedRooms(response.bookedRooms);
            } else {
                handleXhrError(xhr);
            }
        };
        xhr.send(body);
    }

    async function populateReservations() {      
        const xhr = new XMLHttpRequest();
        xhr.open("GET", serverUrl + "admin/reservations");
        setXhrRequestHeaders(xhr);
        xhr.onload = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                console.log("Success, response: " + JSON.stringify(xhr.responseText));
                const response = JSON.parse(xhr.responseText);
                setAllReservations(response.bookedRooms);
            } else {
                handleXhrError(xhr);
            }
        };
        xhr.send();
    }

    async function updateReservation(reservationId) {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", serverUrl + "admin/update");
        setXhrRequestHeaders(xhr);

        const updatedStartDate = document.getElementById("start_reservation" + reservationId).value
        const updatedEndDate = document.getElementById("end_reservation" + reservationId).value
        const body = JSON.stringify({
            ReservationId: reservationId,
            StartDate: !updatedStartDate ? undefined : updatedStartDate,
            EndDate: !updatedEndDate ? undefined : updatedEndDate
        });
        console.log("Body: " + body);
        xhr.onload = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                console.log("Success, response: " + JSON.stringify(xhr.responseText));
                populateReservations();
            } else {
                handleXhrError(xhr);
            }
        };
        xhr.send(body);
    }

    async function cancelReservation(reservationId) {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", serverUrl + "admin/cancel");
        setXhrRequestHeaders(xhr);
        const body = JSON.stringify({
            ReservationId: reservationId,
        });
        xhr.onload = () => {
            if (xhr.readyState == 4 && xhr.status == 200) {
                console.log("Success, response: " + JSON.stringify(xhr.responseText));
                populateReservations();
            } else {
                handleXhrError(xhr);
            }
        };
        xhr.send(body);
    }

    function handleXhrError(xhr) {
        try {
            alert(`Error: ${xhr.status}, message: ${JSON.stringify(JSON.parse(xhr.responseText).errors)}`);
        } catch (error) {
            alert(`Error: ${xhr.status}, message: ${xhr.responseText}`);
        }
    }

    function setXhrRequestHeaders(xhr) {
        xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8")
        xhr.setRequestHeader("X-Road-Client", "DEV/COM/222/TESTCLIENT")
    }
}

export default App;